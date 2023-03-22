﻿using CS6232_G2.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CS6232_G2.DAL
{
    public class AppointmentDAL
    {
        public List<Doctor> GetAllDoctors()
        {
            List<Doctor> doctors = new List<Doctor>();

            string selectStatement = "select d.doctorId, u.firstName + ' ' + u.lastName as doctorName " +
                "from Doctors d left join Users u on d.userId = u.userId";

            using (SqlConnection connection = G2ProjectConnectionString.GetConnection())
            {
                connection.Open();

                using (SqlCommand selectCommand = new SqlCommand(selectStatement, connection))
                {
                    using (SqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Doctor doctor = new Doctor
                            {
                                DoctorId = Convert.ToInt32(reader["DoctorId"]),
                                DoctorName = reader["DoctorName"].ToString()
                            };

                            doctors.Add(doctor);
                        }
                    }
                }
            }

            return doctors;
        }

        /// <summary>
        /// Insert or update appointment by id
        /// </summary>
        /// <param name="appointment"></param>
        /// <returns></returns>
        public bool SaveAppointment(Appointment appointment)
        {
            string query = appointment.AppointmentId <= 0 ?
                "Insert into Appointments(patientId, doctorId, appointmentTime, reasonsForVisit) " +
                "Values(@patientId, @doctorId, @appointmentTime, @reason) "
                :
                "Update Appointments " +
                "Set doctorId = @doctorId, appointmentTime = @appointmentTime, reasonsForVisit = @reason " +
                "Where appointmentId = @appointmentId and doctorId = @doctorId";

            using (SqlConnection connection = G2ProjectConnectionString.GetConnection())
            {
                connection.Open();

                using (SqlCommand saveCommand = new SqlCommand(query, connection))
                {
                    DateTime appointmentTime = appointment.AppointmentTime.Value.AddSeconds(appointment.AppointmentTime.Value.Second * -1);
                    
                    saveCommand.Parameters.AddWithValue("@patientId", appointment.PatientId);
                    saveCommand.Parameters.AddWithValue("@doctorId", appointment.DoctorId);
                    saveCommand.Parameters.AddWithValue("@appointmentTime", appointmentTime);
                    saveCommand.Parameters.AddWithValue("@reason", appointment.Reason);

                    if (appointment.AppointmentId > 0)
                    {
                        saveCommand.Parameters.AddWithValue("@appointmentId", appointment.AppointmentId);
                    }

                    int rowsAffected = saveCommand.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        /// <summary>
        /// Gets the appointment details from the database by appointment id
        /// </summary>
        /// <param name="appointmentId"></param>
        /// <returns></returns>
        public Appointment GetAppointmentById(int appointmentId)
        {
            Appointment appointment = null;

            string selectStatement = "SELECT [appointmentId],[doctorId],[patientId],[appointmentTime] ,[reasonsForVisit], u.firstName + ' ' + u.lastName as patientName " +
                "FROM [Appointments] a left join users u on u.userId = a.patientId " +
                "Where appointmentId = @appointmentId";

            using (SqlConnection connection = G2ProjectConnectionString.GetConnection())
            {
                connection.Open();

                using (SqlCommand selectCommand = new SqlCommand(selectStatement, connection))
                {
                    selectCommand.Parameters.AddWithValue("@appointmentId", appointmentId);

                    using (SqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            appointment = new Appointment
                            {
                                AppointmentId = appointmentId,
                                DoctorId = Convert.ToInt32(reader["doctorId"]),
                                AppointmentTime = Convert.ToDateTime(reader["appointmentTime"]),
                                PatientId = Convert.ToInt32(reader["patientId"]),
                                PatientName = reader["patientName"].ToString(),
                                Reason = reader["reasonsForVisit"].ToString(),
                            };
                        }
                    }
                }
            }

            return appointment;
        }

        /// <summary>
        /// Looks for the appointment date and time and determine if the doctor is already booked
        /// </summary>
        /// <param name="doctorId"></param>
        /// <param name="appointmentTime"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool IsDoctorAvailable(int doctorId, DateTime appointmentTime)
        {
            string selectStatement = "SELECT count(*) AppointmentsCount " +
                "FROM [Appointments] " +
                "Where doctorId = @doctorId and appointmentTime = @appointmentTime";

            using (SqlConnection connection = G2ProjectConnectionString.GetConnection())
            {
                connection.Open();

                using (SqlCommand selectCommand = new SqlCommand(selectStatement, connection))
                {
                    selectCommand.Parameters.AddWithValue("@doctorId", doctorId);
                    selectCommand.Parameters.AddWithValue("@appointmentTime", appointmentTime.ToString("yyyy-MM-dd HH:mm"));

                    var numberOfAppointments = Convert.ToInt32(selectCommand.ExecuteScalar());

                    return numberOfAppointments > 0 ? false : true;
                }
            }
        }
    }
}