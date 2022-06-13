using HIS.Core.DoctorModel;
using HIS.Core.PatientModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.AppointmentModel.Util
{
    /// <summary>
    /// Information relevant to automatically finding an appointment fitting certain criteria.
    /// </summary>
    public class AppointmentSearchBundle
    {
        public static Predicate<TimeSpan> TsInDay = ts => ts >= TimeSpan.FromHours(0) && ts < TimeSpan.FromHours(24);
        public static Predicate<TimeSpan> TsZeroSeconds = ts => ts.Seconds == 0;
        public static Func<TimeSpan, TimeSpan, bool> TsIsAfter = (ts1, ts2) => ts1 > ts2;
        public static Predicate<DateTime> DtNotTooSoon = dt => (dt.Date - DateTime.Today).TotalDays >= 1;

        public const string ErrTimeSpanNotInDay = "TimeSpan must be between 00:00:00 and 23:59:59";
        public const string ErrTimeSpanHasSeconds = "Timespan's seconds component must be zero";
        public const string ErrEndBeforeStart = "End of range must be after start of range";
        public const string ErrDateTooSoon = "Latest date must be at least a day after today";

        public readonly static Doctor DefaultDoctor = null;
        public readonly static Patient DefaultPatient = null;
        public readonly static TimeSpan DefaultStart = TimeSpan.FromHours(0);
        public readonly static TimeSpan DefaultEnd = TimeSpan.FromHours(24) - TimeSpan.FromMinutes(1);
        public readonly static DateTime DefaultBy = DateTime.MaxValue;

        public static bool Urgent = false;
        public Doctor Doctor { get; set; }

        public Patient Patient { get; set; }

        private TimeSpan _start;
        public TimeSpan Start
        {
            get { return _start; }
            set
            {
                if (!TsInDay(value)) throw new ArgumentException(ErrTimeSpanNotInDay);
                if (!TsZeroSeconds(value)) throw new ArgumentException(ErrTimeSpanHasSeconds);
                _start = value;
            }
        }

        private TimeSpan _end;
        public TimeSpan End
        {
            get { return _end; }
            set
            {
                if (!TsInDay(value)) throw new ArgumentException(ErrTimeSpanNotInDay);
                if (!TsZeroSeconds(value)) throw new ArgumentException(ErrTimeSpanHasSeconds);
                if (!TsIsAfter(value, Start)) throw new ArgumentException(ErrEndBeforeStart);
                _end = value;
            }
        }

        private DateTime _by;
        public DateTime By
        {
            get { return _by; }
            set
            {
                if (!DtNotTooSoon(value) && !Urgent) throw new ArgumentException(ErrDateTooSoon);
                _by = value;
            }
        }

        public AppointmentSearchBundle(Doctor doctor, Patient patient, TimeSpan start, TimeSpan end, DateTime by)
        {
            Doctor = doctor;
            Patient = patient;
            Start = start;
            End = end;
            By = by;
        }

        public AppointmentSearchBundle(Doctor doctor, Patient patient, TimeSpan start, TimeSpan end, DateTime by, bool urgent)
        {
            Urgent = urgent;
            Doctor = doctor;
            Patient = patient;
            Start = start;
            End = end;
            By = by;
        }

        public AppointmentSearchBundle() : this(DefaultDoctor, DefaultPatient, DefaultStart, DefaultEnd, DefaultBy)
        {

        }

        public AppointmentSearchBundle(AppointmentSearchBundle other) : this(other.Doctor, other.Patient, other.Start, other.End, other.By)
        {

        }

        public static AppointmentSearchBundle IgnoreInterval(AppointmentSearchBundle sb)
        {
            return new AppointmentSearchBundle(sb)
            {
                Start = DefaultStart,
                End = DefaultEnd
            };
        }

        public static AppointmentSearchBundle IgnoreDoctor(AppointmentSearchBundle sb)
        {
            return new AppointmentSearchBundle(sb)
            {
                Doctor = DefaultDoctor
            };
        }

        public static AppointmentSearchBundle IgnoreLatestDate(AppointmentSearchBundle sb)
        {
            return new AppointmentSearchBundle(sb)
            {
                By = DefaultBy
            };
        }


        public static AppointmentSearchBundle RespectOnlyLatestDateAndPatient(AppointmentSearchBundle sb)
        {
            return new AppointmentSearchBundle(sb)
            {
                Doctor = DefaultDoctor,
                Start = DefaultStart,
                End = DefaultEnd
            };
        }

        public static AppointmentSearchBundle RespectOnlyPatient(AppointmentSearchBundle sb)
        {
            return new AppointmentSearchBundle(sb)
            {
                Doctor = DefaultDoctor,
                Start = DefaultStart,
                End = DefaultEnd,
                By = DefaultBy
            };
        }

        public static AppointmentSearchBundle RespectOnlyDoctorAndPatient(AppointmentSearchBundle sb)
        {
            return new AppointmentSearchBundle(sb)
            {
                Start = DefaultStart,
                End = DefaultEnd,
                By = DefaultBy
            };
        }

        public static AppointmentSearchBundle RespectOnlyIntervalAndPatient(AppointmentSearchBundle sb)
        {
            return new AppointmentSearchBundle(sb)
            {
                Doctor = DefaultDoctor,
                By = DefaultBy
            };
        }
    }
}
