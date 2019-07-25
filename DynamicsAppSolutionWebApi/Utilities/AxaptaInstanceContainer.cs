using Microsoft.Dynamics.BusinessConnectorNet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;

namespace DynamicsAppSolutionWebApi.Utilities
{
    public static class AxaptaInstanceContainer
    {
        public static Axapta GetInstance(string userName, string password, string domain, string company)
        {
            AxaptaInstance axapta = new AxaptaInstance();

            if (axapta.Get(userName, password, domain, "") == null)
            {
                throw new Exception("It's impossible to instanciate AX");
            }

            return axapta.Get(userName, password, domain, company);
        }

        private class AxaptaInstance
        {
            /// <summary>
            /// Instanciated Axapta and get instance
            /// </summary>
            /// <param name="userName"></param>
            /// <param name="password"></param>
            /// <param name="domain"></param>
            /// <param name="company"></param>
            /// <returns></returns>
            public Axapta Get(string userName, string password, string domain, string company)
            {
                if (String.IsNullOrEmpty(userName))
                {
                    throw new Exception("Username is null.");
                }
                if (String.IsNullOrEmpty(password))
                {
                    throw new Exception("Password is null.");
                }
                if (String.IsNullOrEmpty(domain))
                {
                    throw new Exception("Domain is null.");
                }
               

                Axapta axapta = new Axapta();
                NetworkCredential networkCredential = new NetworkCredential(userName, password, domain);
                if (company == "Organization-Wide")
                    company = string.Empty;
                axapta.LogonAs(userName, domain, networkCredential, company, ConfigurationManager.AppSettings["language"], ConfigurationManager.AppSettings["objectServer"], ConfigurationManager.AppSettings["configuration"]);
                return axapta;
            }
        }

    }
}