using CS6232_G2.Controller;
using CS6232_G2.Model;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CS6232_G2
{
    public partial class AppointmentForm : Form
    {
        private DateTime _lastTimeValue;
        private AppointmentController _appointmentController;
        private Appointment _appointment;
        private List<Doctor> _doctorList;

        /// <summary>
        /// Constructor to draw the ui components and initialize the controller
        /// </summary>
        public AppointmentForm(Appointment appointment)
        {
            InitializeComponent();
            _appointmentController = new AppointmentController();
            _appointment = appointment;
        }

        private void AppointmentForm_Load(object sender, EventArgs e)
        {
            if (_appointment.PatientId == 0 || string.IsNullOrEmpty(_appointment.PatientName))
            {
                MessageBox.Show("Invalid request, please select a patient first", "Invalid patient", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbDoctors.Enabled = false;
                dtAppointmentDate.Enabled = false;
                dtAppointmentTime.Enabled = false;
                txtReason.Enabled = false;
                return;
            }

            DateTime newAvailableTime = CalculateNextAvailableTime(DateTime.Now);
            _lastTimeValue = newAvailableTime;
            dtAppointmentDate.MinDate = newAvailableTime;
            dtAppointmentTime.MinDate = newAvailableTime;

            BindDoctors();
            BindAppointmentValues();
        }

        private void BindDoctors()
        {
            try
            {
                _doctorList = _appointmentController.GetDoctors();
                cbDoctors.Items.Clear();
                cbDoctors.Items.AddRange(_doctorList.ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
            }
        }

        private void BindAppointmentValues()
        {
            lblPatientName.Text = _appointment.PatientName;

            if (_appointment.AppointmentId > 0)
            {
                //try
                //{
                //    _appointment = _appointmentController.GetAppointmentById(_appointment.AppointmentId);
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message, ex.GetType().ToString());
                //}

                dtAppointmentDate.Value = _appointment.AppointmentTime;
                dtAppointmentTime.Value = _appointment.AppointmentTime;
                txtReason.Text = _appointment.Reason;

                if (_doctorList?.Count > 0)
                {
                    cbDoctors.SelectedItem = _doctorList.Find(d => d.DoctorId == _appointment.DoctorId);
                }
            }
        }

        private DateTime CalculateNextAvailableTime(DateTime baseTime)
        {
            DateTime nextAvailable;

            if (baseTime.Minute == 0)
            {
                nextAvailable = baseTime;
            }
            else if (baseTime.Minute > 0 & baseTime.Minute <= 15)
            {
                nextAvailable = baseTime.AddMinutes(15 - baseTime.Minute);
            }
            else if (baseTime.Minute >= 16 & baseTime.Minute <= 30)
            {
                nextAvailable = baseTime.AddMinutes(30 - baseTime.Minute);
            }
            else if (baseTime.Minute >= 31 & baseTime.Minute <= 45)
            {
                nextAvailable = baseTime.AddMinutes(45 - baseTime.Minute);
            }
            else
            {
                nextAvailable = baseTime.AddMinutes(60 - baseTime.Minute);
            }

            return nextAvailable;
        }

        private void dtAppointmentTime_ValueChanged(object sender, EventArgs e)
        {
            if (_lastTimeValue == dtAppointmentTime.Value)
            {
                return;
            }

            TimeSpan elapsed = new TimeSpan(dtAppointmentTime.Value.Ticks - _lastTimeValue.Ticks);
            int minuteIncrements = 0;

            if (elapsed.TotalMinutes > 59)
            {
                _lastTimeValue = dtAppointmentTime.Value;
                return;
            }
            else if (elapsed.TotalMinutes == 59)
            {
                minuteIncrements = _lastTimeValue < dtAppointmentTime.Value.AddHours(-1) ? 74 : -74;
            }
            else
            {
                minuteIncrements = _lastTimeValue < dtAppointmentTime.Value ? 14 : -14;
            }

            DateTime newValue = dtAppointmentTime.Value.AddMinutes(minuteIncrements);

            newValue = newValue < dtAppointmentTime.MinDate ? dtAppointmentTime.MinDate : newValue;
            newValue = CalculateNextAvailableTime(newValue);
            _lastTimeValue = newValue;
            dtAppointmentTime.Value = newValue;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (IsFormValid())
            {
                Doctor selectedDoctor = (Doctor)cbDoctors.SelectedItem;
                int doctorId = selectedDoctor.DoctorId;

                _appointment.DoctorId = doctorId;
                _appointment.AppointmentTime = dtAppointmentDate.Value.Date + dtAppointmentTime.Value.TimeOfDay;
                _appointment.Reason = txtReason.Text.Trim();

                try
                {
                    if (_appointmentController.SaveAppointment(_appointment))
                    {
                        MessageBox.Show("Appointment has been saved", "Save");
                    }
                    else
                    {
                        MessageBox.Show("Failed to save appointment, re-open page and try again", "Unable to save");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().ToString());
                }
            }
        }

        private bool IsFormValid()
        {
            DateTime apptointmentTime = dtAppointmentDate.Value.Date + dtAppointmentTime.Value.TimeOfDay;

            if (apptointmentTime < DateTime.Now)
            {
                MessageBox.Show("Please choose a future time", "Invalid appointment time");
                return false;
            }
            else if (cbDoctors.SelectedItem == null)
            {
                MessageBox.Show("No doctor selected for the appointment", "Please choose a different time");
                return false;
            }
            else if (!IsDoctorAvailability())
            {
                MessageBox.Show("Doctor is not available", "Please choose a different time");
                return false;
            }
            else if (txtReason.Text.Trim().Length == 0)
            {
                MessageBox.Show("Pleasae enter a reason for the visit", "Reason is required");
                return false;
            }

            return true;
        }

        private bool IsDoctorAvailability()
        {
            // validate if the doctor is book
            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}