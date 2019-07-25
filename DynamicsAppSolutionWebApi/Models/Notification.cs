using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicsAppSolutionWebApi.Models
{
    public class Notification
    {
        public string AlertedFor { get; set; }

        public string SubjectNotification { get; set; }

        public string DateTimeNotification { get; set; }

        public string DueDateTime { get; set; }

        public int IsRead { get; set; }

        public string Message { get; set; }

        public double RecIdNotification { get; set; }

        public string CompanyId { get; set; }

        public string NotificationType { get; set; }
    }
}