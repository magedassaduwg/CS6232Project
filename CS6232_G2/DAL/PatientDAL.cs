﻿using CS6232_G2.Model;
using System;
using System.Data.SqlClient;

namespace CS6232_G2.DAL
{
    /// <summary>
    /// The DAL for interacting with the Patients table
    /// </summary>
    public class PatientDAL
    {
        private static Patient patientToEdit;

        /// <summary>
        /// Returns the patient to be edited
        /// </summary>
        /// <returns></returns>
        public Patient GetPatientToEdit()
        {
            return patientToEdit;
        }

        /// <summary>
        /// sets the patient to be edited
        /// </summary>
        /// <returns></returns>
        public void SetPatientToEdit(Patient patient)
        {
            patientToEdit = patient;

        }

        /// <summary>
        /// Adds a new patient to the database
        /// </summary>
        /// <param name="user"></param>
        public void AddPatient(User user)
        {
            using (SqlConnection connection = G2ProjectConnectionString.GetConnection())
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                SqlCommand insertUserCommand = new SqlCommand("INSERT INTO Users (lastName, firstName, dob, ssn, gender, streetNumber, city, state, phone, zipcode) " +
                "VALUES (@lastName, @firstName, @dob, @ssn, @gender, @streetNumber, @city, @state, @phone, @zipcode) " +
                "SELECT scope_identity() as lastUserId ", connection, transaction);
                insertUserCommand.Parameters.AddWithValue("@lastName", user.LastName);
                insertUserCommand.Parameters.AddWithValue("@firstName", user.FirstName);
                insertUserCommand.Parameters.AddWithValue("@dob", user.DOB);
                insertUserCommand.Parameters.AddWithValue("@ssn", user.SSN);
                insertUserCommand.Parameters.AddWithValue("@gender", user.Gender);
                insertUserCommand.Parameters.AddWithValue("@streetNumber", user.StreetNumber);
                insertUserCommand.Parameters.AddWithValue("@city", user.City);
                insertUserCommand.Parameters.AddWithValue("@state", user.State);
                insertUserCommand.Parameters.AddWithValue("@phone", user.Phone);
                insertUserCommand.Parameters.AddWithValue("@zipcode", user.Zipcode);

                try
                {
                    var value = insertUserCommand.ExecuteScalar();
                    int lastUser = Convert.ToInt32(value);

                    SqlCommand insertPatientCommand = new SqlCommand("INSERT INTO Patients (UserId) " +
                    "Values (@LastId); ", connection, transaction);
                    insertPatientCommand.Parameters.AddWithValue("@LastId", lastUser);
                    insertPatientCommand.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Updates a user data in the database relating to the patient
        /// </summary>
        /// <param name="user"></param>
        public bool UpdatePatient(User user, User oldUser)
        {
            string insertStatement =
                "UPDATE Users SET " +
                "lastName = @lastName, " +
                "firstName = @firstName, " +
                "dob = @dob, " +
                "ssn = @ssn, " +
                "gender = @gender, " +
                "streetNumber = @streetNumber, " +
                "city = @city, " +
                "state = @state, " +
                "phone = @phone, " +
                "zipcode = @zipcode " +
                "WHERE userId = @oldUserId " +
                "AND lastName = @oldLastName " +
                "AND firstName = @oldFirstName " +
                "AND dob = @oldDob " +
                "AND ssn = @oldSsn " +
                "AND gender = @oldGender " +
                "AND streetNumber = @oldStreetNumber " +
                "AND city = @oldCity " +
                "AND state = @oldState " +
                "AND phone = @oldPhone " +
                "AND zipcode = @oldZipcode ";
            using (SqlConnection connection = G2ProjectConnectionString.GetConnection())
            {
                connection.Open();
                SqlCommand insertCommand = new SqlCommand(insertStatement, connection);
                insertCommand.Parameters.AddWithValue("@lastName", user.LastName);
                insertCommand.Parameters.AddWithValue("@firstName", user.FirstName);
                insertCommand.Parameters.AddWithValue("@dob", user.DOB.ToShortDateString());
                insertCommand.Parameters.AddWithValue("@ssn", user.SSN);
                insertCommand.Parameters.AddWithValue("@gender", user.Gender);
                insertCommand.Parameters.AddWithValue("@streetNumber", user.StreetNumber);
                insertCommand.Parameters.AddWithValue("@city", user.City);
                insertCommand.Parameters.AddWithValue("@state", user.State);
                insertCommand.Parameters.AddWithValue("@phone", user.Phone);
                insertCommand.Parameters.AddWithValue("@zipcode", user.Zipcode);

                insertCommand.Parameters.AddWithValue("@oldUserId", oldUser.UserId);
                insertCommand.Parameters.AddWithValue("@oldLastName", oldUser.LastName);
                insertCommand.Parameters.AddWithValue("@oldFirstName", oldUser.FirstName);
                insertCommand.Parameters.AddWithValue("@oldDob", oldUser.DOB.ToShortDateString());
                insertCommand.Parameters.AddWithValue("@oldSsn", oldUser.SSN);
                insertCommand.Parameters.AddWithValue("@oldGender", oldUser.Gender);
                insertCommand.Parameters.AddWithValue("@oldStreetNumber", oldUser.StreetNumber);
                insertCommand.Parameters.AddWithValue("@oldCity", oldUser.City);
                insertCommand.Parameters.AddWithValue("@oldState", oldUser.State);
                insertCommand.Parameters.AddWithValue("@oldPhone", oldUser.Phone);
                insertCommand.Parameters.AddWithValue("@oldZipcode", oldUser.Zipcode);

                int count = insertCommand.ExecuteNonQuery();
                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Returns the Patient associated with the given patientId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Patient GetPatientByPatientId(int id)
        {
            Patient patient = new Patient();
            string selectStatement =
                        "SELECT userId " +
                        "FROM Patients p " +
                        "WHERE patientId = @patientId";
            using (SqlConnection connection = G2ProjectConnectionString.GetConnection())
            {
                connection.Open();

                using (SqlCommand selectCommand = new SqlCommand(selectStatement, connection))
                {
                    selectCommand.Parameters.AddWithValue("@patientId", id);
                    using (SqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            patient.UserId = int.Parse(reader["userId"].ToString());
                        }
                    }
                }
            }
            return patient;
        }
    }
}
