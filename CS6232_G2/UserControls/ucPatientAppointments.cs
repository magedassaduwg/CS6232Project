﻿using CS6232_G2.Controller;
using CS6232_G2.Model;
using CS6232_G2.View;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CS6232_G2.UserControls
{
    /// <summary>
    /// Patients Appointment user control will display the appointments of the patient and an option to add apointment or edit the user
    /// </summary>
    public partial class ucPatientAppointments : UserControl
    {
        private Patient _user;
        private PatientVisit _appoinment;
        private UserController _userController;
        private RoutineCheckController _routineCheckController;
        private AppointmentController _appointmentController;
        private List<Appointment> _appointments = new List<Appointment>();

        /// <summary>
        /// Constructor to initialize the user control
        /// </summary>
        public ucPatientAppointments()
        {
            InitializeComponent();
            _userController = new UserController();
            _appointmentController = new AppointmentController();
            _routineCheckController= new RoutineCheckController();
            dgAppointments.AutoGenerateColumns = false;
        }

        /// <summary>
        /// Set the userId of the patient selected
        /// </summary>
        /// <param name="userId"></param>
        public void SetPatient(Patient patient)
        {
            _user = patient;
            InitializeAppointment();
        }
        public void SetAppoinment( PatientVisit appoinment)
        {
            _appoinment = appoinment;
            InitializeAppointment();
        }
        private void InitializeAppointment()
        {
            try
            {
                GetPatientAppointments();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
            }
        }

        private void GetPatientAppointments()
        {
            try
            {
                _appointments = _appointmentController.GetPatientAppointments(_user.PatientId);

                lblPatientName.Text = $"{_user.FirstName} {_user.LastName}";

                dgAppointments.DataSource = _appointments;

                if (dgAppointments.Rows.Count > 0)
                {
                    dgAppointments.Rows[0].Selected = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
            }
        }

        private void btnEditPatient_Click(object sender, EventArgs e)
        {
            using (EditPatientForm _editForm = new EditPatientForm(_user.UserId))
            {
                _editForm.ShowDialog();
            }
        }

        private void btnViewAppointment_Click(object sender, EventArgs e)
        {
            if (dgAppointments.SelectedRows.Count > 0)
            {
                Appointment appointment = (Appointment)dgAppointments.SelectedRows[0].DataBoundItem;

                using (AppointmentForm _appointmentForm = new AppointmentForm(appointment))
                {
                    _appointmentForm.ShowDialog();
                }

                dgAppointments.Refresh();
            }
        }

        private void btnAddAppointment_Click(object sender, EventArgs e)
        {
            Appointment newAppointment = new Appointment()
            {
                PatientId = _user.PatientId,
                PatientName = $"{_user.FirstName} {_user.LastName}"
            };

            using (AppointmentForm appointmentForm = new AppointmentForm(newAppointment))
            {
                appointmentForm.ShowDialog();
            }

            GetPatientAppointments();
        }

        private void routineCheckupButton_Click(object sender, EventArgs e)
        {

            PatientVisit selectedVisit = (PatientVisit)dgAppointments.SelectedRows[0].DataBoundItem;
            PatientVisit checkup = new PatientVisit()
            {
                AppointmentTime= selectedVisit.AppointmentTime,
            };
            using (RoutineCheckupForm checkupForm = new RoutineCheckupForm(checkup))
            {
                this.Hide();
                DialogResult result = checkupForm.ShowDialog();

                if (result == DialogResult.Cancel)
                {
                    this.Show();
                }
            }

        }
    }
}