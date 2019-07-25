using DynamicsAppSolutionWebApi.Models;
using DynamicsAppSolutionWebApi.Utilities;
using Microsoft.Dynamics.BusinessConnectorNet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Services;
using System.Web.Services;

namespace DynamicsAppSolutionWebApi.Controllers
{
    public class WorklistController : Controller
    {
        public WorklistController()
        {
            //Axapta aasd = new Axapta();
            //this.ConnectionStringAdmin = ConfigurationManager.ConnectionStrings["AdminConnectionString"].ConnectionString;

            //GetWorkTypes("RSNETDEVeloper", "bltx6RlWSVW6xmY9KNshMw==", "kbt.local", "-1,AKN,BLT,DAT,KBT,SBT,SNT,TST,TTR,URT", "2019-07-18T14:05:16", "MTdlOWExMmY4MDZkZWYxMmQzZjg4NDJjMzA3M2I1NTM=", false);
        }

        // GET: Worklist
        public ActionResult Index()
        {
            return View();
        }

        //public string ConnectionStringAdmin { get; set; }

        /// <summary>
        /// Get Work Types from axapta
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="domain"></param>
        /// <param name="companies"></param>
        /// <param name="utcTime"></param>
        /// <param name="signatureValue"></param>
        /// <param name="includeItems"></param>
        /// <returns></returns>
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public JsonResult GetWorkTypes(string userName, string password, string domain, string companies, string utcTime, string signatureValue)
        {
            ValidateSignature.Check(userName, password, utcTime, signatureValue);

            using (Axapta ax = AxaptaInstanceContainer.GetInstance(userName, new SecurityHelper().Decrypt(password, true), domain, ""))
            {
                List<WorkType> list = new List<WorkType>();
                if (companies != null && companies.Length > 0)
                {
                    using (AxaptaRecord workflowWorkItemTableRecord = ax.CreateAxaptaRecord("WorkflowWorkItemTable"))
                    {
                        using (AxaptaRecord workflowTrackingStatusTableRecord = ax.CreateAxaptaRecord("WorkflowTrackingStatusTable"))
                        {
                            string query = "select RecId from %1 " +
                                           "where %1.UserId == curUserId() && %1.Status == WorkflowWorkItemStatus::Pending " +
                                           "join DocumentType, ContextTableId, count(RecId) from %2 group by DocumentType, ContextTableId " +
                                           "where %1.CorrelationId == %2.CorrelationId " +
                                           "&& %2.TrackingStatus == WorkflowTrackingStatus::Pending";

                            ax.ExecuteStmt(query, workflowWorkItemTableRecord, workflowTrackingStatusTableRecord);
                            string axRecCount = ax.ToString();
                            while (workflowWorkItemTableRecord.Found == true)
                            {
                                string empty = string.Empty;
                                if (workflowTrackingStatusTableRecord.Found == true)
                                    empty = workflowTrackingStatusTableRecord.get_Field("ContextTableId").ToString();
                                if (DataAreaIncluding.Check(empty, companies))
                                {
                                    object contextTableId = workflowTrackingStatusTableRecord.get_Field("ContextTableId");
                                    object documentType = workflowTrackingStatusTableRecord.get_Field("DocumentType");
                                    object recId = workflowTrackingStatusTableRecord.get_Field("RecId");
                                    object tblName = ax.CallStaticClassMethod("Global", "tableId2Name", recId);

                                    list.Add(new WorkType() { WorkTypeName = documentType.ToString(), WorkTypeCount = Convert.ToInt32(recId) });
                                    workflowWorkItemTableRecord.Next();
                                }
                            }
                        }
                    }
                }
               // return list;
                return Json(list, JsonRequestBehavior.AllowGet);

            }
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public  void Test(string s)
        {

        }
    }
}