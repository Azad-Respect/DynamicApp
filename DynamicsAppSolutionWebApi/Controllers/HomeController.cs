using DynamicsAppSolutionWebApi.Models;
using Microsoft.Dynamics.BusinessConnectorNet;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Services;
using System.Web.Services;
using System.Security;
using System.DirectoryServices.AccountManagement;
using System.Data.SqlClient;

namespace DynamicsAppSolutionWebApi.Controllers
{
    public class HomeController : Controller
    {
        private bool showStackTrace = true;
        private const string EVENT_INBOX_TABLE = "EventInbox";
        private const string LEDGER_JOURNAL_TABLE = "LedgerJournalTable";
        private const string LEDGER_JOURNAL_TRANS = "LedgerJournalTrans";
        private const string ORGANIZATION_WIDE = "Organization-Wide";
        private const string PENDING_APPROVAL_STATUS = "PendingApproval";
        private const string PURCH_LINE = "PurchLine";
        private const string PURCH_REQ_LINE = "PurchReqLine";
        private const string PURCH_REQ_TABLE = "PurchReqTable";
        private const string PURCH_TABLE = "PurchTable";
        private const string SALES_LINE = "SalesLine";
        private const string SALES_TABLE = "SalesTable";
        private const string TSTIMESHEET_TABLE = "TSTimesheetTable";
        private const int TRACKINGCONTEXT_STEP = 4;
        private const int TRACKINGCONTEXT_WORKITEM = 5;
        private const int TRACKINGTYPE_APPROVAL = 4;
        private const int TRACKINGTYPE_CONCLUSION = 8;
        private const int TRACKINGTYPE_CREATED = 9;
        private const int TRACKINGTYPE_DENIED = 33;
        private const int TRACKINGTYPE_REJECTED = 19;
        private const string TRVCASHADVANCE = "TrvCashAdvance";
        private const string TRVEXPTABLE = "TrvExpTable";
        private const string TRVEXPTRANS = "TrvExpTrans";
        private const string TRVREQUISITIONTABLE = "TrvRequisitionTable";
        private const string WORKFLOW_TABLE = "WorkflowWorkItemTable";
        private const string WORKFLOW_TRACKING_COMMENT_TABLE = "WorkflowTrackingCommentTable";
        private const string WORKFLOW_TRACKING_STATUS_TABLE = "WorkflowTrackingStatusTable";
        private const string WORKFLOW_TRACKING_TABLE = "WorkflowTrackingTable";
        public ActionResult Index()
        {
            Axapta aasd = new Axapta();
            this.ConnectionStringAdmin = ConfigurationManager.ConnectionStrings["AdminConnectionString"].ConnectionString;

            GetWorkTypes("RSNETDEVeloper", "bltx6RlWSVW6xmY9KNshMw==", "kbt.local","-1,AKN,BLT,DAT,KBT,SBT,SNT,TST,TTR,URT", "2019-07-18T14:05:16", "MTdlOWExMmY4MDZkZWYxMmQzZjg4NDJjMzA3M2I1NTM=", false);
            return View();
        }

        public string ConnectionStringAdmin { get; set; }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        [WebMethod]
        public void DeleteNotification(
          string userName,
          string password,
          string domain,
          string companies,
          double recId,
          string utcTime,
          string signatureValue)
        {
            this.ValidateSignature(userName, password, utcTime, signatureValue);
            password = new SecurityHelper().Decrypt(password, true);
            Axapta ax = (Axapta)null;
            try
            {
                string firstCompany = this.GetFirstCompany(companies);
                ax = this.GetAxInstance(userName, password, domain, firstCompany);
                this.DeleteNotificationAx(ax, recId);
                ax.Logoff();
                ax.Dispose();
            }
            catch (Exception ex)
            {
                if (ax != null)
                {
                    ax.Logoff();
                    ax.Dispose();
                }
                throw new Exception(string.Format("Error calling method DeleteNotification - {0} - {1}", (object)ex.Message, this.showStackTrace ? (object)ex.StackTrace : (object)string.Empty));
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        public void DeleteNotifications(
          string userName,
          string password,
          string domain,
          string companies,
          string recIds,
          string utcTime,
          string signatureValue)
        {
            this.ValidateSignature(userName, password, utcTime, signatureValue);
            password = new SecurityHelper().Decrypt(password, true);
            Axapta ax = (Axapta)null;
            try
            {
                string firstCompany = this.GetFirstCompany(companies);
                ax = this.GetAxInstance(userName, password, domain, firstCompany);
                string str1 = recIds;
                char[] chArray = new char[1] { '|' };
                foreach (string str2 in str1.Split(chArray))
                {
                    if (!string.IsNullOrEmpty(str2.Trim()))
                        this.DeleteNotificationAx(ax, Convert.ToDouble(str2));
                }
                ax.Logoff();
                ax.Dispose();
            }
            catch (Exception ex)
            {
                if (ax != null)
                {
                    ax.Logoff();
                    ax.Dispose();
                }
                throw new Exception(string.Format("Error calling method DeleteNotifications - {0} - {1}", (object)ex.Message, this.showStackTrace ? (object)ex.StackTrace : (object)string.Empty));
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        public List<DataArea> GetCompanies(
          string userName,
          string utcTime,
          string signatureValue)
        {
            try
            {
                SqlDatabase sqlDatabase = new SqlDatabase(this.ConnectionStringAdmin);
                DbCommand sqlStringCommand = ((Database)sqlDatabase).GetSqlStringCommand("  select * \r\n                                from PortalMember pm\r\n\t\t                             inner join PortalMemberDataArea pmda ON pm.Id = pmda.IdMember\r\n                               WHERE NetworkUser = @userName");
                ((Database)sqlDatabase).AddInParameter(sqlStringCommand, nameof(userName), DbType.String, (object)userName);
                DataSet dataSet = ((Database)sqlDatabase).ExecuteDataSet(sqlStringCommand);
                List<DataArea> dataAreaList = new List<DataArea>();
                foreach (DataRow row in (InternalDataCollectionBase)dataSet.Tables[0].Rows)
                    dataAreaList.Add(new DataArea()
                    {
                        Id = row["DataAreaId"].ToString(),
                        Name = row["DataAreaName"].ToString()
                    });
                return dataAreaList;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error calling method GetCompanies \n {0} \n\n {1}", (object)ex.Message, this.showStackTrace ? (object)ex.StackTrace : (object)string.Empty));
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        public List<DataArea> GetDataAreas(
          string userName,
          string password,
          string domain,
          string companies,
          string utcTime,
          string signatureValue)
        {
            this.ValidateSignature(userName, password, utcTime, signatureValue);
            password = new SecurityHelper().Decrypt(password, true);
            Axapta axapta = (Axapta)null;
            List<DataArea> dataAreaList = new List<DataArea>();
            string firstCompany = this.GetFirstCompany(companies);
            try
            {
                dataAreaList.Add(new DataArea()
                {
                    Id = "-1",
                    Name = "Organization-Wide"
                });
                axapta = this.GetAxInstance(userName, password, domain, firstCompany);
                AxaptaRecord axaptaRecord = axapta.CreateAxaptaRecord("DataArea");
                string str1 = string.Format("select %1 where %1.IsVirtual == 0");
                axapta.ExecuteStmt(str1, axaptaRecord);
                while (axaptaRecord.Found == true)
                {
                    string str2 = axaptaRecord.get_Field("Id").ToString();
                    string str3 = axaptaRecord.get_Field("Name").ToString();
                    axaptaRecord.get_Field("DataAreaId").ToString();
                    dataAreaList.Add(new DataArea()
                    {
                        Id = str2,
                        Name = str3
                    });
                    axaptaRecord.Next();
                }
                axapta.Logoff();
                axapta.Dispose();
                return dataAreaList;
            }
            catch (Exception ex)
            {
                if (axapta != null)
                {
                    axapta.Logoff();
                    axapta.Dispose();
                }
                throw new Exception(string.Format("Error calling method GetDataAreas - {0} - {1}", (object)ex.Message, this.showStackTrace ? (object)ex.StackTrace : (object)string.Empty));
            }
        }

        public List<EnumEntity> GetEnums(Axapta ax, string baseEnumName)
        {
            List<EnumEntity> enumEntityList = new List<EnumEntity>();
            for (AxaptaObject axaptaObject1 = (AxaptaObject)((AxaptaObject)ax.CallStaticClassMethod("TreeNode", "findNode", (object)string.Format("\\Data Dictionary\\Base Enums\\{0}", (object)baseEnumName))).Call("AOTfirstChild"); axaptaObject1 != null; axaptaObject1 = (AxaptaObject)axaptaObject1.Call("AOTnextSibling"))
            {
                AxaptaObject axaptaObject2 = (AxaptaObject)ax.CallStaticClassMethod("TreeNode", "findNode", (object)string.Format("\\Data Dictionary\\Base Enums\\{0}\\{1}", (object)baseEnumName, (object)axaptaObject1.Call("AOTname").ToString()));
                enumEntityList.Add(new EnumEntity()
                {
                    EnumValue = axaptaObject2.Call("AOTgetProperty", (object)"EnumValue").ToString(),
                    EnumType = axaptaObject1.Call("AOTname").ToString(),
                    EnumLabel = ax.CallStaticClassMethod("SysLabel", "labelId2String", (object)axaptaObject2.Call("AOTgetProperty", (object)"Label").ToString()).ToString()
                });
            }
            return enumEntityList;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        public List<Expense> GetExpenses(
          string userName,
          string password,
          string domain,
          string companies,
          string utcTime,
          string signatureValue)
        {
            try
            {
                this.ValidateSignature(userName, password, utcTime, signatureValue);
                password = new SecurityHelper().Decrypt(password, true);
                Axapta axInstance = this.GetAxInstance(userName, password, domain, string.Empty);
                AxaptaRecord axaptaRecord = axInstance.CreateAxaptaRecord("TrvExpTable");
                string str = string.Format("select %1 order by %1.CreatedDateTime desc where %1.CreatedBy == curuserid()");
                axInstance.ExecuteStmt(str, axaptaRecord);
                List<Expense> expenseList = new List<Expense>();
                while (axaptaRecord.Found == true)
                {
                    Expense expense = new Expense();
                    expense.Name = axaptaRecord.get_Field("ExpNumber").ToString();
                    expense.CreatedDateTime = axaptaRecord.get_Field("CreatedDateTime").ToString();
                    expense.Location = axaptaRecord.get_Field("Destination").ToString();
                    expense.Purpose = axaptaRecord.get_Field("Txt2").ToString();
                    switch (Convert.ToInt32(axaptaRecord.get_Field("ApprovalStatus")))
                    {
                        case 0:
                            expense.Status = "None";
                            break;
                        case 1:
                            expense.Status = "Draft";
                            break;
                        case 2:
                            expense.Status = "Submitted";
                            break;
                        case 3:
                            expense.Status = "Approved";
                            break;
                        case 4:
                            expense.Status = "Rejected";
                            break;
                        case 5:
                            expense.Status = "Ready for transfer";
                            break;
                        case 6:
                            expense.Status = "Processed for payment";
                            break;
                        case 8:
                            expense.Status = "Cancelled";
                            break;
                        case 9:
                            expense.Status = "In Review";
                            break;
                        default:
                            expense.Status = string.Empty;
                            break;
                    }
                    expenseList.Add(expense);
                    axaptaRecord.Next();
                }
                axInstance.Logoff();
                axInstance.Dispose();
                return expenseList;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error calling method GetExpenses \n {0} \n\n {1}", (object)ex.Message, this.showStackTrace ? (object)ex.StackTrace : (object)string.Empty));
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        public List<ExpenseTxt> GetExpenseText(
          string userName,
          string password,
          string domain,
          string companies,
          string utcTime,
          string signatureValue)
        {
            try
            {
                this.ValidateSignature(userName, password, utcTime, signatureValue);
                password = new SecurityHelper().Decrypt(password, true);
                Axapta axInstance = this.GetAxInstance(userName, password, domain, string.Empty);
                AxaptaRecord axaptaRecord = axInstance.CreateAxaptaRecord("TrvTravelTxt");
                string str = string.Format("select %1");
                axInstance.ExecuteStmt(str, axaptaRecord);
                List<ExpenseTxt> expenseTxtList = new List<ExpenseTxt>();
                while (axaptaRecord.Found == true)
                {
                    expenseTxtList.Add(new ExpenseTxt()
                    {
                        Text = axaptaRecord.get_Field("TXT").ToString()
                    });
                    axaptaRecord.Next();
                }
                axInstance.Logoff();
                axInstance.Dispose();
                return expenseTxtList;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error calling method GetExpenseText \n {0} \n\n {1}", (object)ex.Message, this.showStackTrace ? (object)ex.StackTrace : (object)string.Empty));
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        public List<Attachment> GetFiles(
          string userName,
          string password,
          string domain,
          string company,
          string recId,
          string utcTime,
          string signatureValue)
        {
            return this.GetFilesFromRecIdAndAlertId(userName, password, domain, company, recId, "0", utcTime, signatureValue);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        public List<Attachment> GetFilesFromRecIdAndAlertId(
          string userName,
          string password,
          string domain,
          string company,
          string recId,
          string alertId,
          string utcTime,
          string signatureValue)
        {
            Axapta ax = (Axapta)null;
            try
            {
                this.ValidateSignature(userName, password, utcTime, signatureValue);
                password = new SecurityHelper().Decrypt(password, true);
                ax = this.GetAxInstance(userName, password, domain, company);
                AxaptaRecord axaptaRecord1 = ax.CreateAxaptaRecord("WorkflowWorkItemTable");
                string str1 = !(alertId != "0") || alertId == null || !(alertId != "null") ? string.Format("select %1 order by %1.RefTableId, %1.RecId desc where %1.RecId == {0} && %1.UserId == curuserid() ", (object)recId) : string.Format("select %1 order by %1.RefTableId, %1.RecId desc where %1.NotificationId == {0} && %1.UserId == curuserid() ", (object)alertId);
                ax.ExecuteStmt(str1, axaptaRecord1);
                List<Attachment> attachmentList = new List<Attachment>();
                if (axaptaRecord1.Found == true)
                {
                    object field1 = axaptaRecord1.get_Field("RefTableId");
                    object field2 = axaptaRecord1.get_Field("RefRecId");
                    AxaptaRecord axaptaRecord2 = ax.CreateAxaptaRecord("DocuRef");
                    string str2 = string.Format("Select %1 where %1.RefTableId == {0} && %1.RefRecId == {1}", field1, field2);
                    ax.ExecuteStmt(str2, axaptaRecord2);
                    int num = 0;
                    while (axaptaRecord2.Found == true)
                    {
                        string str3 = axaptaRecord2.Call("CompleteFilename").ToString();
                        Attachment attachment = new Attachment();
                        attachment.RecId = Convert.ToDouble(axaptaRecord2.get_Field("RecId"));
                        attachment.FileName = axaptaRecord2.get_Field("Name").ToString();
                        attachment.CreatedDateTime = this.GetDateApplyTimezone(ax, Convert.ToDateTime(axaptaRecord2.get_Field("CreatedDateTime"))).ToString();
                        attachment.FileType = axaptaRecord2.get_Field("TypeId").ToString();
                        attachment.CreatedBy = axaptaRecord2.get_Field("createdBy").ToString();
                        if (str3 != string.Empty)
                        {
                            if (System.IO.File.Exists(str3))
                            {
                                FileInfo fileInfo = new FileInfo(str3);
                                attachment.FileSize = (fileInfo.Length / 1024L).ToString("N0") + " KB";
                            }
                            else
                                attachment.FileSize = this.getFileSize(ax, axaptaRecord2);
                        }
                        else
                            attachment.FileSize = "0 KB";
                        attachmentList.Add(attachment);
                        axaptaRecord2.Next();
                        ++num;
                    }
                }
                ax.Logoff();
                ax.Dispose();
                return attachmentList;
            }
            catch (Exception ex)
            {
                if (ax != null)
                {
                    ax.Logoff();
                    ax.Dispose();
                }
                throw new Exception(string.Format("Error calling method GetFiles \n {0} \n\n {1}", (object)ex.Message, this.showStackTrace ? (object)ex.StackTrace : (object)string.Empty));
            }
        }

        private string getFileSize(Axapta ax, AxaptaRecord docuRef)
        {
            AxaptaRecord axaptaRecord = ax.CreateAxaptaRecord("DocuValue");
            string str = string.Format("Select %1 where %1.RecId == {0}", docuRef.get_Field("ValueRecId"));
            ax.ExecuteStmt(str, axaptaRecord);
            if (axaptaRecord.Found == true)
            {
                object field = axaptaRecord.get_Field("File");
                if (field != null)
                {
                    try
                    {
                        object[] objArray = new object[1] { field };
                        return (Convert.FromBase64String(ax.CallStaticClassMethod("OnTheGoHelper", "GetBase64Str", objArray).ToString()).Length / 1024).ToString("N0") + " KB";
                    }
                    catch
                    {
                        return "0";
                    }
                }
            }
            return "0 KB";
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        [WebMethod]
        public List<WorkItem> GetHistorical(
          string userName,
          string password,
          string domain,
          string companies,
          string utcTime,
          string signatureValue)
        {
            this.ValidateSignature(userName, password, utcTime, signatureValue);
            password = new SecurityHelper().Decrypt(password, true);
            return this.GetHistoricalByStatus(userName, password, domain, companies, new int?(), (string)null);
        }

        public List<WorkItem> GetHistoricalByStatus(
          string userName,
          string password,
          string domain,
          string companies,
          int? status,
          string tableLabel)
        {
            Axapta ax = (Axapta)null;
            try
            {
                List<WorkItem> workItemList = new List<WorkItem>();
                tableLabel = HttpUtility.UrlDecode(tableLabel);
                ax = this.GetAxInstance(userName, password, domain, string.Empty);
                AxaptaRecord axaptaRecord1 = ax.CreateAxaptaRecord("WorkflowTrackingStatusTable");
                string str1 = status.HasValue ? string.Format("select %1 order by %1.RecId desc where %1.TrackingStatus == {0}", (object)status) : string.Format("select %1 order by %1.RecId desc");
                ax.ExecuteStmt(str1, axaptaRecord1);
                int num = 1;
                int result = 30;
                int.TryParse(ConfigurationManager.AppSettings["maxItemsHistorical"], out result);
                while (axaptaRecord1.Found == true)
                {
                    AxaptaRecord axaptaRecord2 = ax.CreateAxaptaRecord("WorkflowTrackingStatusTable");
                    string str2 = string.Format("select %1 where %1.CorrelationId == str2guid('{0}')", axaptaRecord1.get_Field("CorrelationId"));
                    ax.ExecuteStmt(str2, axaptaRecord2);
                    string empty = string.Empty;
                    if (axaptaRecord2.Found == true)
                        empty = axaptaRecord2.get_Field("ContextCompanyId").ToString();
                    if (this.IsDataAreaIncluded(empty, companies))
                    {
                        object field1 = axaptaRecord1.get_Field("ContextTableId");
                        object field2 = axaptaRecord1.get_Field("ContextRecId");
                        object field3 = axaptaRecord1.get_Field("RecId");
                        object obj = ax.CallStaticClassMethod("Global", "tableId2Name", field1);
                        AxaptaRecord axaptaRecord3 = ax.CreateAxaptaRecord(obj.ToString());
                        string str3 = string.Format("select  %1 where %1.RecId == {0}", field2, (object)empty);
                        ax.ExecuteStmt(str3, axaptaRecord3);
                        if (axaptaRecord3.Found == true)
                        {
                            string tableLabel1 = this.GetTableLabel(ax, Convert.ToInt32(field1));
                            if (string.IsNullOrEmpty(tableLabel) || tableLabel1 == tableLabel)
                            {
                                WorkItem workItem = new WorkItem();
                                workItem.RecId = Convert.ToDouble(field3.ToString());
                                workItem.Name = axaptaRecord3.Caption;
                                workItem.DataAreaId = empty == string.Empty ? "Organization-Wide" : empty;
                                workItem.DateTime = this.GetDateApplyTimezone(ax, Convert.ToDateTime(axaptaRecord1.get_Field("CreatedDateTime"))).ToString();
                                workItem.Subject = string.Empty;
                                workItem.Description = string.Empty;
                                workItem.TableName = obj.ToString();
                                workItem.TableLabel = tableLabel1;
                                workItem.Status = string.Empty;
                                workItem.Requisitioner = this.GetEmployeeName(ax, this.GetEmployeeId(ax, axaptaRecord1, empty));
                                workItem.ImageURL = this.GetOriginatorImage(ax, axaptaRecord1, empty);
                                this.GetWorkItemItems(ax, axaptaRecord1, field1, field2, axaptaRecord3, obj.ToString(), workItem, empty);
                                workItem.HistoricalItems = this.GetHistoricalItems(ax, axaptaRecord1.get_Field("CorrelationId").ToString());
                                switch (Convert.ToInt32(axaptaRecord1.get_Field("TrackingStatus")))
                                {
                                    case 0:
                                        workItem.Status = "Pending";
                                        break;
                                    case 1:
                                        workItem.Status = "Completed";
                                        break;
                                    case 2:
                                        workItem.Status = "Cancelled";
                                        break;
                                    case 3:
                                        workItem.Status = "Error";
                                        break;
                                    case 4:
                                        workItem.Status = "Unrecoverable";
                                        break;
                                }
                                workItem.Type = string.Empty;
                                this.VerifyAttachments(ax, axaptaRecord1, field1, field2, workItem);
                                workItemList.Add(workItem);
                                ++num;
                                if (num > result)
                                    break;
                            }
                        }
                    }
                    axaptaRecord1.Next();
                }
                ax.Logoff();
                ax.Dispose();
                return workItemList;
            }
            catch (Exception ex)
            {
                if (ax != null)
                {
                    ax.Logoff();
                    ax.Dispose();
                }
                throw new Exception(string.Format("Error calling method GetHistorical - {0} - {1}", (object)ex.Message, this.showStackTrace ? (object)ex.StackTrace : (object)string.Empty));
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        public List<WorkItem> GetHistoricalByStatus(
          string userName,
          string password,
          string domain,
          string companies,
          int? status,
          string tableLabel,
          string utcTime,
          string signatureValue)
        {
            this.ValidateSignature(userName, password, utcTime, signatureValue);
            password = new SecurityHelper().Decrypt(password, true);
            return this.GetHistoricalByStatus(userName, password, domain, companies, status, tableLabel);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        [WebMethod]
        public WorkItem GetHistoricalDetail(
          string userName,
          string password,
          string domain,
          string companies,
          string utcTime,
          string signatureValue,
          string recId)
        {
            this.ValidateSignature(userName, password, utcTime, signatureValue);
            password = new SecurityHelper().Decrypt(password, true);
            Axapta ax = (Axapta)null;
            try
            {
                WorkItem workItem = new WorkItem();
                ax = this.GetAxInstance(userName, password, domain, string.Empty);
                AxaptaRecord axaptaRecord1 = ax.CreateAxaptaRecord("WorkflowTrackingStatusTable");
                string str1 = string.Format("select %1 order by %1.RecId desc where %1.RecId == {0}", (object)recId);
                ax.ExecuteStmt(str1, axaptaRecord1);
                while (axaptaRecord1.Found == true)
                {
                    AxaptaRecord axaptaRecord2 = ax.CreateAxaptaRecord("WorkflowTrackingStatusTable");
                    string str2 = string.Format("select %1 where %1.CorrelationId == str2guid('{0}')", axaptaRecord1.get_Field("CorrelationId"));
                    ax.ExecuteStmt(str2, axaptaRecord2);
                    string empty = string.Empty;
                    if (axaptaRecord2.Found == true)
                        empty = axaptaRecord2.get_Field("ContextCompanyId").ToString();
                    if (this.IsDataAreaIncluded(empty, companies))
                    {
                        object field1 = axaptaRecord1.get_Field("ContextTableId");
                        object field2 = axaptaRecord1.get_Field("ContextRecId");
                        object field3 = axaptaRecord1.get_Field("RecId");
                        object obj = ax.CallStaticClassMethod("Global", "tableId2Name", field1);
                        AxaptaRecord axaptaRecord3 = ax.CreateAxaptaRecord(obj.ToString());
                        string str3 = string.Format("select  %1 where %1.RecId == {0}", field2, (object)empty);
                        ax.ExecuteStmt(str3, axaptaRecord3);
                        if (axaptaRecord3.Found == true)
                        {
                            workItem.RecId = Convert.ToDouble(field3.ToString());
                            workItem.Name = axaptaRecord3.Caption;
                            workItem.DateTime = this.GetDateApplyTimezone(ax, Convert.ToDateTime(axaptaRecord1.get_Field("CreatedDateTime"))).ToString();
                            workItem.DataAreaId = empty == string.Empty ? "Organization-Wide" : empty;
                            workItem.Subject = string.Empty;
                            workItem.Description = string.Empty;
                            workItem.TableName = obj.ToString();
                            workItem.TableLabel = this.GetTableLabel(ax, Convert.ToInt32(field1));
                            workItem.Requisitioner = string.Empty;
                            workItem.Status = string.Empty;
                            workItem.HistoricalItems = this.GetHistoricalItems(ax, axaptaRecord1.get_Field("CorrelationId").ToString());
                            workItem.ImageURL = this.GetOriginatorImage(ax, axaptaRecord1, empty);
                            this.GetWorkItemItems(ax, axaptaRecord1, field1, field2, axaptaRecord3, obj.ToString(), workItem, empty);
                            switch (Convert.ToInt32(axaptaRecord1.get_Field("TrackingStatus")))
                            {
                                case 0:
                                    workItem.Status = "Pending";
                                    break;
                                case 1:
                                    workItem.Status = "Completed";
                                    break;
                                case 2:
                                    workItem.Status = "Cancelled";
                                    break;
                                case 3:
                                    workItem.Status = "Error";
                                    break;
                                case 4:
                                    workItem.Status = "Unrecoverable";
                                    break;
                            }
                            workItem.Type = string.Empty;
                            this.VerifyAttachments(ax, axaptaRecord1, field1, field2, workItem);
                        }
                    }
                    axaptaRecord1.Next();
                }
                ax.Logoff();
                ax.Dispose();
                return workItem;
            }
            catch (Exception ex)
            {
                if (ax != null)
                {
                    ax.Logoff();
                    ax.Dispose();
                }
                throw new Exception(string.Format("Error calling method GetHistorical - {0} - {1}", (object)ex.Message, this.showStackTrace ? (object)ex.StackTrace : (object)string.Empty));
            }
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        [WebMethod]
        public ItemsCount GetItemsCount(
          string userName,
          string password,
          string domain,
          string companies,
          string utcTime,
          string signatureValue)
        {
            this.ValidateSignature(userName, password, utcTime, signatureValue);
            password = new SecurityHelper().Decrypt(password, true);
            Axapta axapta = (Axapta)null;
            int num1 = 0;
            int num2 = 0;
            try
            {
                List<WorkItem> workItemList = new List<WorkItem>();
                string[] tablesRemove = this.GetTablesRemove();
                if (companies != null && companies.Length > 0)
                {
                    axapta = this.GetAxInstance(userName, password, domain, string.Empty);
                    AxaptaRecord axaptaRecord1 = axapta.CreateAxaptaRecord("WorkflowWorkItemTable");
                    string str1 = "select %1 order by %1.RefTableId, %1.RecId desc where %1.UserId == curuserid() && %1.Status == WorkflowWorkItemStatus::Pending &&   %1.Type ==  WorkflowWorkItemType::WorkItem";
                    axapta.ExecuteStmt(str1, axaptaRecord1);
                    while (axaptaRecord1.Found == true)
                    {
                        AxaptaRecord axaptaRecord2 = axapta.CreateAxaptaRecord("WorkflowTrackingStatusTable");
                        string str2 = string.Format("select %1 where %1.CorrelationId == str2guid('{0}')", axaptaRecord1.get_Field("CorrelationId"));
                        axapta.ExecuteStmt(str2, axaptaRecord2);
                        string empty = string.Empty;
                        if (axaptaRecord2.Found == true)
                            empty = axaptaRecord2.get_Field("ContextCompanyId").ToString();
                        if (this.IsDataAreaIncluded(empty, companies))
                        {
                            object field = axaptaRecord1.get_Field("RefTableId");
                            object tblName = axapta.CallStaticClassMethod("Global", "tableId2Name", field);
                            if (!this.inTablesRemove(tablesRemove, tblName))
                                ++num1;
                        }
                        axaptaRecord1.Next();
                    }
                    string firstCompany = this.GetFirstCompany(companies);
                    /*axapta = this.GetAxInstance(userName, password, domain, firstCompany);
                    AxaptaRecord axaptaRecord3 = axapta.CreateAxaptaRecord("EventInbox");
                    string str3 = "select %1 order by %1.RecId desc where %1.UserId == curuserid() && %1.Deleted == 0";
                    axapta.ExecuteStmt(str3, axaptaRecord3);
                    while (axaptaRecord3.Found == true)
                    {
                        axaptaRecord3.Next();
                        ++num2;
                        if (num2 > 100)
                            break;
                    }
                    axapta.Logoff();
                    axapta.Dispose();
                    */
                }
            }
            catch (Exception ex)
            {
                if (axapta != null)
                {
                    axapta.Logoff();
                    axapta.Dispose();
                }
                throw new Exception(string.Format("Error calling method GetItemsCount - {0} - {1}", (object)ex.Message, this.showStackTrace ? (object)ex.StackTrace : (object)string.Empty));
            }
            return new ItemsCount()
            {
                WorkItemsCount = num1,
                NotificationsCount = num2
            };
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        public List<Notification> GetNotifications(
          string userName,
          string password,
          string domain,
          string companies,
          string utcTime,
          string signatureValue)
        {
            this.ValidateSignature(userName, password, utcTime, signatureValue);
            password = new SecurityHelper().Decrypt(password, true);
            Axapta ax = (Axapta)null;
            try
            {
                string firstCompany = this.GetFirstCompany(companies);
                List<Notification> notificationList = new List<Notification>();
                ax = this.GetAxInstance(userName, password, domain, firstCompany);
                AxaptaRecord axaptaRecord = ax.CreateAxaptaRecord("EventInbox");
                string str1 = "select %1 order by %1.RecId desc where %1.UserId == curuserid() && %1.Deleted == 0";
                ax.ExecuteStmt(str1, axaptaRecord);
                int num = 0;
                while (axaptaRecord.Found == true)
                {
                    Notification notification1 = new Notification();
                    notification1.Message = (string)axaptaRecord.get_Field("Message");
                    notification1.SubjectNotification = (string)axaptaRecord.get_Field("Subject");
                    notification1.AlertedFor = (string)axaptaRecord.get_Field("AlertedFor");
                    notification1.IsRead = (int)axaptaRecord.get_Field("IsRead");
                    notification1.RecIdNotification = Convert.ToDouble(axaptaRecord.get_Field("RecId").ToString());
                    Notification notification2 = notification1;
                    DateTime dateTime1 = this.GetDateApplyTimezone(ax, Convert.ToDateTime(axaptaRecord.get_Field("AlertCreatedDateTime")));
                    string str2 = dateTime1.ToString();
                    notification2.DateTimeNotification = str2;
                    DateTime dateTime2 = new DateTime(1900, 1, 1);
                    if (Convert.ToDateTime(axaptaRecord.get_Field("DueDateTime")) == dateTime2)
                    {
                        notification1.DueDateTime = string.Empty;
                    }
                    else
                    {
                        Notification notification3 = notification1;
                        dateTime1 = Convert.ToDateTime(axaptaRecord.get_Field("DueDateTime"));
                        string shortDateString = dateTime1.ToShortDateString();
                        notification3.DueDateTime = shortDateString;
                    }
                    notification1.CompanyId = (string)axaptaRecord.get_Field("CompanyId");
                    notification1.NotificationType = this.GetEnumLabelByEnumValue(ax, "EventNotificationType", Convert.ToInt32(axaptaRecord.get_Field("NotificationType")));
                    notificationList.Add(notification1);
                    axaptaRecord.Next();
                    ++num;
                    if (num > 100)
                        break;
                }
                ax.Logoff();
                ax.Dispose();
                return notificationList;
            }
            catch (Exception ex)
            {
                if (ax != null)
                {
                    ax.Logoff();
                    ax.Dispose();
                }
                throw new Exception(string.Format("Error calling method GetNotifications - {0} - {1}", (object)ex.Message, this.showStackTrace ? (object)ex.StackTrace : (object)string.Empty));
            }
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        [WebMethod]
        public Notification GetNotificationsById(
          string userName,
          string password,
          string domain,
          string companies,
          string utcTime,
          string signatureValue,
          string recId)
        {
            this.ValidateSignature(userName, password, utcTime, signatureValue);
            password = new SecurityHelper().Decrypt(password, true);
            Axapta ax = (Axapta)null;
            try
            {
                string firstCompany = this.GetFirstCompany(companies);
                ax = this.GetAxInstance(userName, password, domain, firstCompany);
                AxaptaRecord axaptaRecord = ax.CreateAxaptaRecord("EventInbox");
                string str1 = "select %1 order by %1.RecId desc where %1.UserId == curuserid() && %1.Recid == " + recId;
                ax.ExecuteStmt(str1, axaptaRecord);
                Notification notification1 = new Notification();
                if (axaptaRecord.Found == true)
                {
                    notification1.Message = (string)axaptaRecord.get_Field("Message");
                    notification1.SubjectNotification = (string)axaptaRecord.get_Field("Subject");
                    notification1.AlertedFor = (string)axaptaRecord.get_Field("AlertedFor");
                    notification1.IsRead = (int)axaptaRecord.get_Field("IsRead");
                    notification1.RecIdNotification = Convert.ToDouble(axaptaRecord.get_Field("RecId").ToString());
                    Notification notification2 = notification1;
                    DateTime dateTime = Convert.ToDateTime(axaptaRecord.get_Field("AlertCreatedDateTime"));
                    string str2 = dateTime.ToString();
                    notification2.DateTimeNotification = str2;
                    Notification notification3 = notification1;
                    dateTime = Convert.ToDateTime(axaptaRecord.get_Field("DueDateTime"));
                    string str3 = dateTime.ToString();
                    notification3.DueDateTime = str3;
                    notification1.CompanyId = (string)axaptaRecord.get_Field("CompanyId");
                    notification1.NotificationType = this.GetEnumLabelByEnumValue(ax, "EventNotificationType", Convert.ToInt32(axaptaRecord.get_Field("NotificationType")));
                }
                ax.Logoff();
                ax.Dispose();
                return notification1;
            }
            catch (Exception ex)
            {
                if (ax != null)
                {
                    ax.Logoff();
                    ax.Dispose();
                }
                throw new Exception(string.Format("Error calling method GetNotificationsById - {0} - {1}", (object)ex.Message, this.showStackTrace ? (object)ex.StackTrace : (object)string.Empty));
            }
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        [WebMethod]
        public int GetNotificationsCount(
          string userName,
          string password,
          string domain,
          string companies,
          string utcTime,
          string signatureValue)
        {
            this.ValidateSignature(userName, password, utcTime, signatureValue);
            password = new SecurityHelper().Decrypt(password, true);
            Axapta axapta = (Axapta)null;
            try
            {
                string firstCompany = this.GetFirstCompany(companies);
                axapta = this.GetAxInstance(userName, password, domain, firstCompany);
                AxaptaRecord axaptaRecord = axapta.CreateAxaptaRecord("EventInbox");
                string str = "select %1 order by %1.RecId desc where %1.UserId == curuserid() && %1.Deleted == 0";
                axapta.ExecuteStmt(str, axaptaRecord);
                int num = 0;
                while (axaptaRecord.Found == true)
                {
                    axaptaRecord.Next();
                    ++num;
                    if (num > 100)
                        break;
                }
                axapta.Logoff();
                axapta.Dispose();
                return num;
            }
            catch (Exception ex)
            {
                if (axapta != null)
                {
                    axapta.Logoff();
                    axapta.Dispose();
                }
                throw new Exception(string.Format("Error calling method GetNotifications - {0} - {1}", (object)ex.Message, this.showStackTrace ? (object)ex.StackTrace : (object)string.Empty));
            }
        }

        public int GetTotalTransactions(string userName)
        {
            SqlDatabase sqlDatabase = new SqlDatabase(this.ConnectionStringAdmin);
            DbCommand sqlStringCommand = ((Database)sqlDatabase).GetSqlStringCommand("select TransactionsCount = count(*)\r\n                                from PortalMember pm\r\n\t                                 inner join PortalMemberTransactions pmt ON pm.Id = pmt.IdMember\r\n                               WHERE NetworkUser = @userName");
            ((Database)sqlDatabase).AddInParameter(sqlStringCommand, nameof(userName), DbType.String, (object)userName);
            DataSet dataSet = ((Database)sqlDatabase).ExecuteDataSet(sqlStringCommand);
            if (dataSet.Tables[0].Rows.Count > 0)
                return Convert.ToInt32(dataSet.Tables[0].Rows[0]["TransactionsCount"]);
            return 0;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        public List<User> GetUserList(
          string userName,
          string password,
          string domain,
          string companies,
          string recId,
          string type,
          string utcTime,
          string signatureValue)
        {
            this.ValidateSignature(userName, password, utcTime, signatureValue);
            password = new SecurityHelper().Decrypt(password, true);
            List<User> userList = new List<User>();
            Axapta axapta = (Axapta)null;
            try
            {
                string firstCompany = this.GetFirstCompany(companies);
                axapta = this.GetAxInstance(userName, password, domain, firstCompany);
                string str1 = recId.Split(',')[0];
                StringBuilder stringBuilder = new StringBuilder();
                if (type == this.GetActionTypeFromString("Delegate").ToString())
                {
                    AxaptaRecord axaptaRecord1 = axapta.CreateAxaptaRecord("WorkflowWorkItemTable");
                    string str2 = string.Format("Select %1 where %1.RecId == {0} ", (object)str1);
                    axapta.ExecuteStmt(str2, axaptaRecord1);
                    if (axaptaRecord1.Found == true)
                    {
                        AxaptaRecord axaptaRecord2 = axapta.CreateAxaptaRecord("WorkflowWorkItemTable");
                        string str3 = string.Format("Select %1 where %1.CorrelationId == str2guid('{0}') && %1.StepId == str2guid('{1}') && %1.Status == WorkflowWorkItemStatus::Pending", axaptaRecord1.get_Field("CorrelationId"), axaptaRecord1.get_Field("StepId"));
                        axapta.ExecuteStmt(str3, axaptaRecord2);
                        while (axaptaRecord2.Found == true)
                        {
                            stringBuilder.AppendFormat("%1.ID != '{0}' &&", axaptaRecord2.get_Field("UserId"));
                            axaptaRecord2.Next();
                        }
                        if (stringBuilder.Length > 0)
                            stringBuilder.Remove(stringBuilder.Length - 2, 2);
                    }
                }
                else
                    stringBuilder.Append("%1.ID != curuserid()");
                AxaptaRecord axaptaRecord = axapta.CreateAxaptaRecord("UserInfo");
                string str4 = string.Format("Select %1 ", (object)stringBuilder);
                axapta.ExecuteStmt(str4, axaptaRecord);
                while (axaptaRecord.Found == true)
                {
                    userList.Add(new User()
                    {
                        UserId = axaptaRecord.get_Field("ID").ToString(),
                        UserName = axaptaRecord.get_Field("NAME").ToString()
                    });
                    axaptaRecord.Next();
                }
                axapta.Logoff();
                axapta.Dispose();
            }
            catch (Exception ex)
            {
                if (axapta != null)
                {
                    axapta.Logoff();
                    axapta.Dispose();
                }
                throw new Exception(string.Format("Error calling method GetUserList - {0} - {1}", (object)ex.Message, this.showStackTrace ? (object)ex.StackTrace : (object)string.Empty));
            }
            return userList;
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        [WebMethod]
        public UserMain GetUserMain(string userName, string password, string utcTime, string signatureValue)
        {
            try
            {
                SqlDatabase sqlDatabase = new SqlDatabase(this.ConnectionStringAdmin);
                DbCommand sqlStringCommand = ((Database)sqlDatabase).GetSqlStringCommand("  select pa.*, pm.Language\r\n                                from PortalMember pm\r\n\t                                 inner join PortalAccount pa on pm.IdAccount = pa.Id\r\n                               WHERE NetworkUser = @userName");
                ((Database)sqlDatabase).AddInParameter(sqlStringCommand, nameof(userName), DbType.String, (object)userName);
                DataSet dataSet = ((Database)sqlDatabase).ExecuteDataSet(sqlStringCommand);
                UserMain userMain = new UserMain();
                SecurityHelper securityHelper = new SecurityHelper();
                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    DataRow row = dataSet.Tables[0].Rows[0];
                    userMain.Logo = ConfigurationManager.AppSettings["UploadPath"] + "/" + row["Logo"].ToString();
                    if (dataSet.Tables[0].Rows[0]["U"] != DBNull.Value)
                        userMain.LicensedTo = securityHelper.Decrypt(row["U"].ToString(), true);
                    userMain.Companies = this.GetCompanies(userName, utcTime, signatureValue);
                    userMain.UserCompleteName = "User Complete Name";
                    userMain.UserImage = "/UserImg/1.png";
                    userMain.IsLicenseValid = true;
                    userMain.Domain = row["Domain"].ToString();
                    userMain.Language = row["Language"].ToString();
                    userMain.URLWebService = row["URLWebService"].ToString();
                    userMain.QuantityTransactions = this.GetTotalTransactions(userName);
                }
                else
                    userMain.Logo = ConfigurationManager.AppSettings["UploadPath"] + "/OnTheGo.png";
                return userMain;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error calling method GetCompanies \n {0} \n\n {1}", (object)ex.Message, this.showStackTrace ? (object)ex.StackTrace : (object)string.Empty));
            }
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        [WebMethod]
        public string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        public WorkItem GetWorkItemDetail(
          string userName,
          string password,
          string domain,
          string company,
          string utcTime,
          string signatureValue,
          string recId,
          string alertId)
        {
            this.ValidateSignature(userName, password, utcTime, signatureValue);
            password = new SecurityHelper().Decrypt(password, true);
            Axapta ax = (Axapta)null;
            WorkItem workItem = new WorkItem();
            try
            {
                ax = this.GetAxInstance(userName, password, domain, company);
                AxaptaRecord axaptaRecord1 = ax.CreateAxaptaRecord("WorkflowWorkItemTable");
                string str1 = !(alertId != "0") ? string.Format("select %1 order by %1.RefTableId, %1.RecId desc where %1.RecId == {0} && %1.UserId == curuserid() ", (object)recId) : string.Format("select %1 order by %1.RefTableId, %1.RecId desc where %1.NotificationId == {0} && %1.UserId == curuserid() ", (object)alertId);
                ax.ExecuteStmt(str1, axaptaRecord1);
                while (axaptaRecord1.Found == true)
                {
                    object field1 = axaptaRecord1.get_Field("RefTableId");
                    object field2 = axaptaRecord1.get_Field("RefRecId");
                    object field3 = axaptaRecord1.get_Field("RecId");
                    object obj = ax.CallStaticClassMethod("Global", "tableId2Name", field1);
                    AxaptaRecord axaptaRecord2 = ax.CreateAxaptaRecord(obj.ToString());
                    string str2 = string.Format("select %1 where %1.RecId == {0}", field2);
                    ax.ExecuteStmt(str2, axaptaRecord2);
                    while (axaptaRecord2.Found == true)
                    {
                        string tableLabel = this.GetTableLabel(ax, Convert.ToInt32(field1));
                        axaptaRecord1.get_Field("ConfigurationId").ToString();
                        string company1 = company;
                        try
                        {
                            company1 = axaptaRecord2.get_Field("DataAreaId").ToString();
                        }
                        catch
                        {
                        }
                        workItem = this.GetWorkItemDet(ax, axaptaRecord1, field1, field2, field3, axaptaRecord2, tableLabel, obj.ToString(), company1);
                        axaptaRecord2.Next();
                    }
                    axaptaRecord1.Next();
                }
                ax.Logoff();
                ax.Dispose();
                return workItem;
            }
            catch (Exception ex)
            {
                if (ax != null)
                {
                    ax.Logoff();
                    ax.Dispose();
                }
                throw new Exception(string.Format("Error calling method GetWorkItem - {0} - {1}", (object)ex.Message, this.showStackTrace ? (object)ex.StackTrace : (object)string.Empty));
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        public List<WorkItem> GetWorkItems(
          string userName,
          string password,
          string domain,
          string companies,
          string utcTime,
          string signatureValue)
        {
            return this.GetWorkItems(userName, password, domain, companies, utcTime, signatureValue, false);
        }

        public List<WorkItem> GetWorkItems(
          string userName,
          string password,
          string domain,
          string companies,
          string utcTime,
          string signatureValue,
          bool includeItems)
        {
            this.ValidateSignature(userName, password, utcTime, signatureValue);
            password = new SecurityHelper().Decrypt(password, true);
            Axapta ax = (Axapta)null;
            try
            {
                List<WorkItem> list = new List<WorkItem>();
                string[] tablesRemove = this.GetTablesRemove();
                if (companies != null && companies.Length > 0)
                {
                    ax = this.GetAxInstance(userName, password, domain, "");
                    AxaptaRecord axaptaRecord1 = ax.CreateAxaptaRecord("WorkflowWorkItemTable");
                    string str1 = "select %1 order by %1.RefTableId, %1.RecId desc where %1.UserId == curuserid() \r\n                            && %1.Status == WorkflowWorkItemStatus::Pending &&   %1.Type ==  WorkflowWorkItemType::WorkItem";
                    ax.ExecuteStmt(str1, axaptaRecord1);
                    int num = 1;
                    while (axaptaRecord1.Found == true)
                    {
                        AxaptaRecord axaptaRecord2 = ax.CreateAxaptaRecord("WorkflowTrackingStatusTable");
                        string str2 = string.Format("select %1 where %1.CorrelationId == str2guid('{0}')", axaptaRecord1.get_Field("CorrelationId"));
                        ax.ExecuteStmt(str2, axaptaRecord2);
                        string empty = string.Empty;
                        if (axaptaRecord2.Found == true)
                            empty = axaptaRecord2.get_Field("ContextCompanyId").ToString();
                        if (this.IsDataAreaIncluded(empty, companies))
                        {
                            object field1 = axaptaRecord1.get_Field("RefTableId");
                            object field2 = axaptaRecord1.get_Field("RefRecId");
                            object field3 = axaptaRecord1.get_Field("RecId");
                            object tblName = ax.CallStaticClassMethod("Global", "tableId2Name", field1);
                            if (!this.inTablesRemove(tablesRemove, tblName))
                            {
                                AxaptaRecord axaptaRecord3 = ax.CreateAxaptaRecord(tblName.ToString());
                                string str3 = !string.IsNullOrEmpty(empty) ? string.Format("container c = ['{1}']; select crosscompany: c * from %1 where %1.RecId == {0}", field2, (object)empty) : string.Format("select * from %1 where %1.RecId == {0}", field2);
                                ax.ExecuteStmt(str3, axaptaRecord3);
                                if (axaptaRecord3.Found == true)
                                {
                                    string tableLabel = this.GetTableLabel(ax, Convert.ToInt32(field1));
                                    axaptaRecord1.get_Field("ConfigurationId").ToString();
                                    list.Add(this.GetWorkItem(ax, axaptaRecord1, field1, field2, field3, axaptaRecord3, tableLabel, tblName.ToString(), empty, includeItems));
                                }
                            }
                        }
                        axaptaRecord1.Next();
                        ++num;
                    }
                    ax.Logoff();
                    ax.Dispose();
                }
                list.Sort<WorkItem>("TableLabel asc, ActionsString asc, RecId desc");
                return list;
            }
            catch (Exception ex)
            {
                if (ax != null)
                {
                    ax.Logoff();
                    ax.Dispose();
                }
                throw new Exception(string.Format("Error calling method GetWorkItems - {0} - {1}", (object)ex.Message, this.showStackTrace ? (object)ex.StackTrace : (object)string.Empty));
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        public int GetWorkItemsCount(
          string userName,
          string password,
          string domain,
          string companies,
          string utcTime,
          string signatureValue)
        {
            this.ValidateSignature(userName, password, utcTime, signatureValue);
            password = new SecurityHelper().Decrypt(password, true);
            Axapta axapta = (Axapta)null;
            int num = 0;
            try
            {
                List<WorkItem> workItemList = new List<WorkItem>();
                string[] tablesRemove = this.GetTablesRemove();
                if (companies != null && companies.Length > 0)
                {
                    axapta = this.GetAxInstance(userName, password, domain, string.Empty);
                    AxaptaRecord axaptaRecord1 = axapta.CreateAxaptaRecord("WorkflowWorkItemTable");
                    string str1 = "select %1 order by %1.RefTableId, %1.RecId desc where %1.UserId == curuserid() && %1.Status == WorkflowWorkItemStatus::Pending &&   %1.Type ==  WorkflowWorkItemType::WorkItem";
                    axapta.ExecuteStmt(str1, axaptaRecord1);
                    while (axaptaRecord1.Found == true)
                    {
                        AxaptaRecord axaptaRecord2 = axapta.CreateAxaptaRecord("WorkflowTrackingStatusTable");
                        string str2 = string.Format("select %1 where %1.CorrelationId == str2guid('{0}')", axaptaRecord1.get_Field("CorrelationId"));
                        axapta.ExecuteStmt(str2, axaptaRecord2);
                        string empty = string.Empty;
                        if (axaptaRecord2.Found == true)
                            empty = axaptaRecord2.get_Field("ContextCompanyId").ToString();
                        if (this.IsDataAreaIncluded(empty, companies))
                        {
                            object field = axaptaRecord1.get_Field("RefTableId");
                            object tblName = axapta.CallStaticClassMethod("Global", "tableId2Name", field);
                            if (!this.inTablesRemove(tablesRemove, tblName))
                                ++num;
                        }
                        axaptaRecord1.Next();
                    }
                    axapta.Logoff();
                    axapta.Dispose();
                }
                return num;
            }
            catch (Exception ex)
            {
                if (axapta != null)
                {
                    axapta.Logoff();
                    axapta.Dispose();
                }
                throw new Exception(string.Format("Error calling method GetWorkItems - {0} - {1}", (object)ex.Message, this.showStackTrace ? (object)ex.StackTrace : (object)string.Empty));
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        public List<WorkItem> GetWorkItemsWithItems(
          string userName,
          string password,
          string domain,
          string companies,
          string utcTime,
          string signatureValue)
        {
            return this.GetWorkItems(userName, password, domain, companies, utcTime, signatureValue, true);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        [WebMethod]
        public void NotificationMarkAsReadUnRead(
          string userName,
          string password,
          string domain,
          string companies,
          double recId,
          bool isRead,
          string utcTime,
          string signatureValue)
        {
            this.ValidateSignature(userName, password, utcTime, signatureValue);
            password = new SecurityHelper().Decrypt(password, true);
            Axapta axapta = (Axapta)null;
            try
            {
                string firstCompany = this.GetFirstCompany(companies);
                axapta = this.GetAxInstance(userName, password, domain, firstCompany);
                AxaptaRecord axaptaRecord = axapta.CreateAxaptaRecord("EventInbox");
                string str = string.Format("select forupdate %1 where %1.RecId == {0}", (object)recId);
                axapta.ExecuteStmt(str, axaptaRecord);
                if (axaptaRecord.Found == true)
                {
                    axapta.TTSBegin();
                    axaptaRecord.set_Field("IsRead", (object)(isRead ? 1 : 0));
                    axaptaRecord.Update();
                    axapta.TTSCommit();
                }
                axapta.Logoff();
                axapta.Dispose();
            }
            catch (Exception ex)
            {
                if (axapta != null)
                {
                    axapta.Logoff();
                    axapta.Dispose();
                }
                throw new Exception(string.Format("Error calling method NotificationMarkAsReadUnRead - {0} - {1}", (object)ex.Message, this.showStackTrace ? (object)ex.StackTrace : (object)string.Empty));
            }
        }

        public Bitmap ResizeBitmap(Bitmap b, int nWidth, int nHeight)
        {
            Bitmap bitmap = new Bitmap(nWidth, nHeight);
            using (Graphics graphics = Graphics.FromImage((Image)bitmap))
                graphics.DrawImage((Image)b, 0, 0, nWidth, nHeight);
            return bitmap;
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        [WebMethod]
        public string SaveCompanies(
          string userName,
          string password,
          string dataAreaIds,
          string language)
        {
            try
            {
                password = new SecurityHelper().Decrypt(password, true);
                UserInfo userInfo = this.ValidateUser(userName, password);
                if (userInfo.HasError)
                    throw new Exception(string.Format(userInfo.ErrorDetails));
                int num = 0;
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                string str = dataAreaIds;
                char[] chArray = new char[1] { Convert.ToChar(",") };
                foreach (string key in str.Split(chArray))
                    dictionary.Add(key, key);
                SqlDatabase sqlDatabase = new SqlDatabase(this.ConnectionStringAdmin);
                DbCommand sqlStringCommand1 = ((Database)sqlDatabase).GetSqlStringCommand("  select pm.*, u.Username from PortalMember pm\r\n\t                                                        left join aspnet_users u on pm.UserId = u.UserId\r\n                                                        where pm.NetworkUser = @userName");
                ((Database)sqlDatabase).AddInParameter(sqlStringCommand1, nameof(userName), DbType.String, (object)userName);
                DataSet ds1 = ((Database)sqlDatabase).ExecuteDataSet(sqlStringCommand1);
                if (ds1.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds1.Tables[0].Rows[0];
                    num = Convert.ToInt32(row["Id"]);
                    row["Language"] = (object)language;
                    this.UpdateMember(ds1);
                }
                ds1.Dispose();
                sqlStringCommand1.Dispose();
                if (num != 0)
                {
                    DbCommand sqlStringCommand2 = ((Database)sqlDatabase).GetSqlStringCommand(" delete from PortalMemberDataArea where IdMember = @idMember");
                    ((Database)sqlDatabase).AddInParameter(sqlStringCommand2, "idMember", DbType.Int32, (object)num);
                    ((Database)sqlDatabase).ExecuteNonQuery(sqlStringCommand2);
                    DbCommand sqlStringCommand3 = ((Database)sqlDatabase).GetSqlStringCommand("  select * \r\n                                from PortalMemberDataArea WHERE idMember = @idMember");
                    ((Database)sqlDatabase).AddInParameter(sqlStringCommand3, "idMember", DbType.Int32, (object)num);
                    DataSet ds2 = ((Database)sqlDatabase).ExecuteDataSet(sqlStringCommand3);
                    ds2.Tables[0].Rows.Clear();
                    foreach (KeyValuePair<string, string> keyValuePair in dictionary)
                    {
                        DataRow row = ds2.Tables[0].NewRow();
                        row["DataAreaId"] = (object)keyValuePair.Key;
                        row["DataAreaName"] = (object)keyValuePair.Value;
                        row["IdMember"] = (object)num;
                        ds2.Tables[0].Rows.Add(row);
                    }
                    this.UpdateDataArea(ds2);
                }
                return "Success";
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error calling method SaveCompanies \n {0} \n\n {1}", (object)ex.Message, this.showStackTrace ? (object)ex.StackTrace : (object)string.Empty));
            }
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        [WebMethod]
        public string UpdateService(
          string userName,
          string password,
          string utcTime,
          string signatureValue)
        {
            try
            {
                this.ValidateSignature(userName, password, utcTime, signatureValue);
                new WebClient().DownloadFile("https://www.dynamicsonthego.com.br/Portal/downloads/DynamicsOnTheGo.WebService.txt", HttpContext.Request.MapPath(HttpContext.Request.ApplicationPath) + "bin\\DynamicsOnTheGo.WebService.dll");
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message + " " + ex.StackTrace + " " + ex.InnerException.ToString() + " " + WindowsIdentity.GetCurrent().Name.ToString();
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        public void UpdateWorkflow(
          string userName,
          string password,
          string domain,
          string company,
          string templateName,
          int action,
          string recId,
          string comments,
          string toUser,
          string utcTime,
          string signatureValue)
        {
            this.ValidateSignature(userName, password, utcTime, signatureValue);
            password = new SecurityHelper().Decrypt(password, true);
            Axapta ax = (Axapta)null;
            bool flag = false;
            try
            {
                if (templateName.StartsWith("EP"))
                    flag = true;
                ax = this.GetAxInstance(userName, password, domain, company);
                string str1 = recId;
                char[] chArray = new char[1] { ',' };
                foreach (string recId1 in str1.Split(chArray))
                {
                    if (templateName == "Cancel")
                    {
                        this.CancelWorkflow(ax, recId1, comments);
                    }
                    else
                    {
                        AxaptaRecord axaptaRecord = ax.CreateAxaptaRecord("WorkflowWorkItemTable");
                        string str2 = string.Format("Select forupdate %1 where %1.RecId == {0} && %1.UserId == curuserid()", (object)recId1);
                        ax.TTSBegin();
                        ax.ExecuteStmt(str2, axaptaRecord);
                        object[] objArray = new object[6]
                        {
              (object) axaptaRecord,
              (object) HttpUtility.UrlDecode(comments),
              (object) toUser,
              (object) action,
              (object) string.Format("{0}", (object) templateName),
              (object) flag
                        };
                        ax.CallStaticClassMethod("WorkflowWorkItemActionManager", "dispatchWorkItemAction", objArray);
                        ax.TTSCommit();
                    }
                }
                ax.Logoff();
                ax.Dispose();
            }
            catch (Exception ex)
            {
                if (ax != null)
                {
                    ax.Logoff();
                    ax.Dispose();
                }
                throw new Exception(string.Format("Error calling method UpdateWorkflow - user: {2} \n {0} \n\n {1} ", (object)ex.Message, this.showStackTrace ? (object)ex.StackTrace : (object)string.Empty, (object)toUser));
            }
        }

        private DataRow GetSystemCredentials()
        {
            SqlDatabase sqlDatabase = new SqlDatabase(this.ConnectionStringAdmin);
            DbCommand sqlStringCommand = ((Database)sqlDatabase).GetSqlStringCommand("select * from PushSetup");
            DataSet dataSet = ((Database)sqlDatabase).ExecuteDataSet(sqlStringCommand);
            if (dataSet.Tables[0].Rows.Count > 0)
                return dataSet.Tables[0].Rows[0];
            return (DataRow)null;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        public string UpdateWorkflowDirect(
          string userName,
          string password,
          string domain,
          int action,
          string alertId,
          string hash)
        {
            Axapta axapta = new Axapta();
            try
            {
                if (HttpUtility.UrlDecode(hash) != HttpUtility.UrlDecode(this.CreateHash(alertId)))
                    return "Invalid hash. Action was not executed. ";
                if (!this.CheckSerialNumber())
                    return "Invalid serial number. Please contact Dynamics On the Go Team.";
                password = new SecurityHelper().Decrypt(password, true);
                axapta = this.GetAxInstance(userName, password, domain, string.Empty);
                string str1 = alertId;
                char[] chArray = new char[1] { ',' };
                foreach (string str2 in str1.Split(chArray))
                {
                    AxaptaRecord axaptaRecord1 = axapta.CreateAxaptaRecord("WorkflowWorkItemTable");
                    string str3 = string.Format("Select forupdate %1 where %1.NotificationId == {0} && %1.UserId == curuserid()", (object)str2);
                    axapta.TTSBegin();
                    axapta.ExecuteStmt(str3, axaptaRecord1);
                    if (!axaptaRecord1.Found == true)
                    {
                        axapta.Logoff();
                        axapta.Dispose();
                        return "Work item does not exist. Maybe you have already taken an action.";
                    }
                    string empty = string.Empty;
                    AxaptaRecord axaptaRecord2 = axapta.CreateAxaptaRecord("WorkflowElementTable");
                    string str4 = string.Format("Select %1 where %1.ElementId == str2guid('{0}')", axaptaRecord1.get_Field("ElementId"));
                    axapta.ExecuteStmt(str4, axaptaRecord2);
                    string str5 = axaptaRecord2.get_Field("ElementName").ToString();
                    int int32 = Convert.ToInt32(axaptaRecord2.get_Field("ElementType"));
                    AxaptaObject axaptaObject1 = (AxaptaObject)axapta.CallStaticClassMethod("TreeNode", "findNode", (object)string.Format("\\Workflow\\{0}\\{1}\\Outcomes", int32 == 0 ? (object)"Approvals" : (object)"Tasks", (object)str5));
                    if (axaptaObject1 != null)
                    {
                        object obj = axaptaObject1.Call("AOTfirstChild");
                        if (obj != null)
                        {
                            for (AxaptaObject axaptaObject2 = (AxaptaObject)obj; axaptaObject2 != null; axaptaObject2 = (AxaptaObject)axaptaObject2.Call("AOTnextSibling"))
                            {
                                if (axaptaObject2.Call("AOTgetProperty", (object)"Enabled") != null && axaptaObject2.Call("AOTgetProperty", (object)"Enabled").ToString() != "No" && (axaptaObject2.Call("AOTgetProperty", (object)"Type") != null && Convert.ToInt32(this.GetActionTypeFromString(axaptaObject2.Call("AOTgetProperty", (object)"Type").ToString())) == action))
                                {
                                    empty = axaptaObject2.Call("AOTgetProperty", (object)"ActionMenuItem").ToString();
                                    break;
                                }
                            }
                        }
                    }
                    if (empty != string.Empty)
                    {
                        object[] objArray = new object[6]
                        {
              (object) axaptaRecord1,
              (object) HttpUtility.UrlDecode("Sent from Dynamics On the Go"),
              (object) string.Empty,
              (object) action,
              (object) string.Format("{0}", (object) empty),
              (object) false
                        };
                        axapta.CallStaticClassMethod("WorkflowWorkItemActionManager", "dispatchWorkItemAction", objArray);
                        axapta.TTSCommit();
                    }
                }
                axapta.Logoff();
                axapta.Dispose();
                return "OK";
            }
            catch (Exception ex)
            {
                if (axapta != null)
                {
                    axapta.Logoff();
                    axapta.Dispose();
                }
                return string.Format("Error calling method UpdateWorkflowDirect - user: {2} \n {0} \n\n {1} ", (object)ex.Message, this.showStackTrace ? (object)ex.StackTrace : (object)string.Empty, (object)string.Empty);
            }
        }

        private bool CheckSerialNumber()
        {
            SqlDatabase sqlDatabase = new SqlDatabase(this.ConnectionStringAdmin);
            DbCommand sqlStringCommand1 = ((Database)sqlDatabase).GetSqlStringCommand("  select * \r\n                                from PortalAccount");
            DataSet ds = ((Database)sqlDatabase).ExecuteDataSet(sqlStringCommand1);
            ds.Tables[0].TableName = "Account";
            DbCommand sqlStringCommand2 = ((Database)sqlDatabase).GetSqlStringCommand("  select * \r\n                                from PortalMember");
            DataSet dataSet = ((Database)sqlDatabase).ExecuteDataSet(sqlStringCommand2);
            dataSet.Tables[0].TableName = "Members";
            ds.Merge(dataSet);
            return this.ValidateHash(ds);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        [WebMethod]
        public void UploadPhoto(
          string userName,
          string password,
          string domain,
          string company,
          string recId,
          string imageBase64,
          string utcTime,
          string signatureValue)
        {
            this.ValidateSignature(userName, password, utcTime, signatureValue);
            password = new SecurityHelper().Decrypt(password, true);
            Axapta axapta = (Axapta)null;
            try
            {
                string str = HttpContext.Request.MapPath(HttpContext.Request.ApplicationPath);
                byte[] buffer = Convert.FromBase64String(imageBase64);
                MemoryStream memoryStream = new MemoryStream(buffer, 0, buffer.Length);
                string filename = str + "ExpenseReport\\" + (object)Guid.NewGuid() + ".jpg";
                memoryStream.Write(buffer, 0, buffer.Length);
                Image.FromStream((Stream)memoryStream, true).Save(filename);
                axapta = this.GetAxInstance(userName, password, domain, company);
                axapta.TTSBegin();
                AxaptaRecord axaptaRecord = axapta.CreateAxaptaRecord("DocuRef");
                axaptaRecord.Call("clear");
                axaptaRecord.set_Field("RefRecId", (object)recId);
                axaptaRecord.set_Field("RefTableId", (object)484);
                axaptaRecord.set_Field("RefCompanyId", (object)"dat");
                axaptaRecord.set_Field("Name", (object)"onthegoazul1.jpg");
                axaptaRecord.set_Field("TypeId", (object)"File");
                axaptaRecord.set_Field("Restriction", (object)0);
                axaptaRecord.Call("insert");
                axapta.CreateAxaptaObject("DocuActionArchive").Call("add", (object)axaptaRecord, (object)filename);
                axapta.TTSCommit();
                axapta.Logoff();
                axapta.Dispose();
            }
            catch (Exception ex)
            {
                if (axapta != null)
                {
                    axapta.Logoff();
                    axapta.Dispose();
                }
                throw new Exception(string.Format("Error calling method UploadPhoto  \n {0} \n\n {1} ", (object)ex.Message, this.showStackTrace ? (object)ex.StackTrace : (object)string.Empty));
            }
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        [WebMethod]
        public void UploadReport(
          string userName,
          string password,
          string domain,
          string company,
          string utcTime,
          string signatureValue,
          string fileName,
          string expNumber,
          string description,
          string amountMST,
          string additionalInformation,
          string expType,
          string legalEntity)
        {
            Axapta ax = (Axapta)null;
            try
            {
                this.ValidateSignature(userName, password, utcTime, signatureValue);
                password = new SecurityHelper().Decrypt(password, true);
                string str = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", string.Empty).Replace("\\bin", string.Empty) + "\\ExpenseReport\\" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".jpg";
                new WebClient().DownloadFile(fileName, str);
                ax = this.GetAxInstance(userName, password, domain, company);
                ax.TTSBegin();
                long travelExpenseItem = this.CreateTravelExpenseItem(ax, expNumber, description, amountMST, additionalInformation, expType, legalEntity, userName);
                this.CreateTravelExpenseDocuRef(company, ax, str, travelExpenseItem);
                ax.TTSCommit();
                ax.Logoff();
                ax.Dispose();
            }
            catch (Exception ex)
            {
                if (ax != null)
                {
                    ax.Logoff();
                    ax.Dispose();
                }
                throw new Exception(string.Format("Error calling method UploadReport  \n {0} \n\n {1} ", (object)ex.Message, (object)ex.StackTrace));
            }
        }

        private void CreateTravelExpenseDocuRef(
          string company,
          Axapta ax,
          string filenameLocal,
          long recId)
        {
            AxaptaRecord axaptaRecord = ax.CreateAxaptaRecord("DocuRef");
            axaptaRecord.Call("clear");
            axaptaRecord.set_Field("RefRecId", (object)recId);
            axaptaRecord.set_Field("RefTableId", (object)484);
            axaptaRecord.set_Field("RefCompanyId", (object)company);
            axaptaRecord.set_Field("Name", (object)"onthegoazul1.jpg");
            axaptaRecord.set_Field("TypeId", (object)"File");
            axaptaRecord.set_Field("Restriction", (object)0);
            axaptaRecord.Call("insert");
            ax.CreateAxaptaObject("DocuActionArchive").Call("add", (object)axaptaRecord, (object)filenameLocal);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        [WebMethod]
        public UserInfo ValidateUserWithToken(
          string userName,
          string password,
          string userDeviceToken)
        {
            UserInfo userInfo = this.ValidateUser(userName, password);
            if (!userInfo.HasError && !string.IsNullOrEmpty(userDeviceToken))
                this.UpdateUserToken(userName, userDeviceToken);
            return userInfo;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        public UserInfo ValidateUser(string userName, string password)
        {
            UserInfo userInfo = new UserInfo();
            userInfo.HasError = false;
            userInfo.ErrorDetails = string.Empty;
            bool flag;
            using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, ConfigurationManager.AppSettings["Domain"]))
                flag = principalContext.ValidateCredentials(userName, password);
            if (!flag)
            {
                userInfo.HasError = true;
                userInfo.ErrorDetails = "Invalid Active Directory username or password.";
            }
            else
            {
                SqlDatabase sqlDatabase = new SqlDatabase(this.ConnectionStringAdmin);
                DbCommand sqlStringCommand1 = ((Database)sqlDatabase).GetSqlStringCommand("  select * \r\n                                from PortalMember pm\t\t                             \r\n                               WHERE NetworkUser = @userName");
                ((Database)sqlDatabase).AddInParameter(sqlStringCommand1, nameof(userName), DbType.String, (object)userName);
                if (((Database)sqlDatabase).ExecuteDataSet(sqlStringCommand1).Tables[0].Rows.Count <= 0)
                {
                    userInfo.HasError = true;
                    userInfo.ErrorDetails = "User not found on the Dynamics AX Workflow Approval application.";
                }
                DbCommand sqlStringCommand2 = ((Database)sqlDatabase).GetSqlStringCommand("  select * \r\n                                from PortalAccount");
                DataSet ds = ((Database)sqlDatabase).ExecuteDataSet(sqlStringCommand2);
                ds.Tables[0].TableName = "Account";
                DbCommand sqlStringCommand3 = ((Database)sqlDatabase).GetSqlStringCommand("  select * \r\n                                from PortalMember");
                DataSet dataSet = ((Database)sqlDatabase).ExecuteDataSet(sqlStringCommand3);
                dataSet.Tables[0].TableName = "Members";
                ds.Merge(dataSet);
                if (!this.ValidateHash(ds))
                {
                    userInfo.HasError = true;
                    userInfo.ErrorDetails = "Invalid Serial Number for the Application. Contact the administrator.";
                }
                SecurityHelper securityHelper = new SecurityHelper();
                userInfo.Password = securityHelper.Encrypt(password, true);
            }
            return userInfo;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        public void UpdateUserToken(string userName, string userDeviceToken)
        {
            if (userDeviceToken.Length > 2)
                userDeviceToken = userDeviceToken.Substring(1, userDeviceToken.Length - 2).Replace(" ", string.Empty);
            SqlDatabase sqlDatabase = new SqlDatabase(this.ConnectionStringAdmin);
            DbCommand sqlStringCommand = ((Database)sqlDatabase).GetSqlStringCommand("  UPDATE PortalMember SET UserDeviceToken = @userDeviceToken\r\n                               WHERE NetworkUser = @userName");
            ((Database)sqlDatabase).AddInParameter(sqlStringCommand, nameof(userName), DbType.String, (object)userName);
            ((Database)sqlDatabase).AddInParameter(sqlStringCommand, nameof(userDeviceToken), DbType.String, (object)userDeviceToken);
            ((Database)sqlDatabase).ExecuteNonQuery(sqlStringCommand);
        }

        private void CancelWorkflow(Axapta ax, string recId, string comments)
        {
            AxaptaRecord axaptaRecord = ax.CreateAxaptaRecord("WorkflowWorkItemTable");
            string str = string.Format("Select %1 where %1.RecId == {0}", (object)recId);
            ax.ExecuteStmt(str, axaptaRecord);
            if (!axaptaRecord.Found == true)
                return;
            object[] objArray = new object[2]
            {
        axaptaRecord.get_Field("CorrelationId"),
        (object) comments
            };
            ax.CallStaticClassMethod("Workflow", nameof(CancelWorkflow), objArray);
        }

        private string CreateHash(string inboxId)
        {
            return Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(inboxId + "9F0DA618-6153-493E-8903-951E9552FDC6")));
        }

        private long CreateTravelExpenseItem(
          Axapta ax,
          string expNumber,
          string description,
          string amountMST,
          string additionalInformation,
          string expType,
          string legalEntity,
          string userName)
        {
            AxaptaRecord axaptaRecord1 = ax.CreateAxaptaRecord("DirPersonUser");
            AxaptaRecord axaptaRecord2 = ax.CreateAxaptaRecord("HcmWorker");
            string str = string.Format("Select * from %1 join %2 where %1.PersonParty == %2.Person && %1.User == '{0}'", (object)userName);
            ax.ExecuteStmt(str, axaptaRecord1, axaptaRecord2);
            if (!axaptaRecord2.Found == true)
                return 0;
            double num = Convert.ToDouble(axaptaRecord2.get_Field("RecId"));
            AxaptaRecord axaptaRecord3 = ax.CreateAxaptaRecord("TrvExpTable");
            axaptaRecord3.set_Field("ExpNumber", (object)expNumber);
            axaptaRecord3.set_Field("LegalEntity", (object)Convert.ToInt64(legalEntity));
            axaptaRecord3.set_Field("ApprovalStatus", (object)1);
            axaptaRecord3.set_Field("CreatingWorker", (object)num);
            axaptaRecord3.Call("insert");
            AxaptaRecord axaptaRecord4 = ax.CreateAxaptaRecord("TrvExpTrans");
            axaptaRecord4.set_Field("ExpNumber", (object)expNumber);
            axaptaRecord4.set_Field("Description", (object)description);
            axaptaRecord4.set_Field("PayMethod", (object)"Cash");
            axaptaRecord4.set_Field("AmountMST", (object)Convert.ToDecimal(amountMST));
            axaptaRecord4.set_Field("AmountCurr", (object)Convert.ToDecimal(amountMST));
            axaptaRecord4.set_Field("AdditionalInformation", (object)additionalInformation);
            axaptaRecord4.set_Field("ExpType", (object)Convert.ToInt32(expType));
            axaptaRecord4.set_Field("CostType", (object)"MEALS");
            axaptaRecord4.set_Field("ExchangeCode", (object)"USD");
            axaptaRecord3.set_Field("ApprovalStatus", (object)1);
            axaptaRecord3.set_Field("CreatingWorker", (object)num);
            axaptaRecord4.set_Field("TrvExpTable", axaptaRecord3.get_Field("RecId"));
            axaptaRecord4.set_Field("LegalEntity", (object)Convert.ToInt64(legalEntity));
            axaptaRecord4.Call("insert");
            return Convert.ToInt64(axaptaRecord3.get_Field("RecId"));
        }

        private void DeleteNotificationAx(Axapta ax, double recId)
        {
            AxaptaRecord axaptaRecord = ax.CreateAxaptaRecord("EventInbox");
            string str = string.Format("select forupdate %1 where %1.RecId == {0}", (object)recId);
            ax.ExecuteStmt(str, axaptaRecord);
            if (!axaptaRecord.Found == true)
                return;
            ax.TTSBegin();
            axaptaRecord.Delete();
            ax.TTSCommit();
        }

        private string GetActionMenuItem(Axapta ax, string templateName, string outcome)
        {
            return ((AxaptaObject)ax.CallStaticClassMethod("TreeNode", "findNode", (object)string.Format("\\Workflow\\Approvals\\{0}\\Outcomes\\{1}", (object)this.GetApprovalNameFromTemplate(ax, templateName), (object)outcome))).Call("AOTgetProperty", (object)"ActionMenuItem").ToString();
        }

        private List<DynamicsAppSolutionWebApi.Models.Action> GetActions(
          Axapta ax,
          bool isClaimed,
          string elementName,
          int elementType)
        {
            List<DynamicsAppSolutionWebApi.Models.Action> actionList = new List<DynamicsAppSolutionWebApi.Models.Action>();
            if (elementType == 1 && !isClaimed)
            {
                actionList.Add(new DynamicsAppSolutionWebApi.Models.Action
                {
                    ActionName = this.GetLabelFromMenuItem(ax, "WorkflowClaimWorkItem"),
                    ActionMenuItem = "WorkflowClaimWorkItem",
                    ActionType = this.GetActionTypeFromString("Claim")
                });
            }
            else
            {
                AxaptaObject axaptaObject1 = (AxaptaObject)ax.CallStaticClassMethod("TreeNode", "findNode", (object)string.Format("\\Workflow\\{0}\\{1}\\Outcomes", elementType == 0 ? (object)"Approvals" : (object)"Tasks", (object)elementName));
                if (axaptaObject1 != null)
                {
                    object obj = axaptaObject1.Call("AOTfirstChild");
                    if (obj != null)
                    {
                        for (AxaptaObject axaptaObject2 = (AxaptaObject)obj; axaptaObject2 != null; axaptaObject2 = (AxaptaObject)axaptaObject2.Call("AOTnextSibling"))
                        {
                            if (axaptaObject2.Call("AOTgetProperty", (object)"Enabled") != null && axaptaObject2.Call("AOTgetProperty", (object)"Enabled").ToString() != "No")
                            {
                                DynamicsAppSolutionWebApi.Models.Action action = new DynamicsAppSolutionWebApi.Models.Action();
                                action.ActionName = string.Empty;
                                action.ActionMenuItem = string.Empty;
                                action.ActionType = -1;
                                if (axaptaObject2.Call("AOTgetProperty", (object)"ActionMenuItem") != null)
                                {
                                    if (axaptaObject2.Call("AOTgetProperty", (object)"ActionMenuItem").ToString() == string.Empty)
                                    {
                                        action.ActionName = axaptaObject2.Call("AOTgetProperty", (object)"Name").ToString();
                                        action.ActionMenuItem = axaptaObject2.Call("AOTgetProperty", (object)"ActionWebMenuItem").ToString();
                                    }
                                    else
                                    {
                                        action.ActionName = this.GetLabelFromMenuItem(ax, axaptaObject2.Call("AOTgetProperty", (object)"ActionMenuItem").ToString());
                                        action.ActionMenuItem = axaptaObject2.Call("AOTgetProperty", (object)"ActionMenuItem").ToString();
                                    }
                                }
                                if (axaptaObject2.Call("AOTgetProperty", (object)"Type") != null)
                                    action.ActionType = this.GetActionTypeFromString(axaptaObject2.Call("AOTgetProperty", (object)"Type").ToString());
                                actionList.Add(action);
                            }
                        }
                    }
                }
            }
            actionList.Add(new DynamicsAppSolutionWebApi.Models.Action()
            {
                ActionName = this.GetLabelFromActionType(ax, "Delegate"),
                ActionMenuItem = "Delegate",
                ActionType = this.GetActionTypeFromString("Delegate")
            });
            actionList.Add(new DynamicsAppSolutionWebApi.Models.Action()
            {
                ActionName = this.GetLabelFromMenuItem(ax, "WorkflowStatusCancel"),
                ActionMenuItem = "Cancel",
                ActionType = 0
            });
            return actionList;
        }

        private int GetActionTypeFromString(string type)
        {
            switch (type)
            {
                case "None":
                    return 0;
                case "Complete":
                    return 1;
                case "Return":
                    return 2;
                case "RequestChange":
                    return 3;
                case "Delegate":
                    return 4;
                case "Resubmit":
                    return 5;
                case "Deny":
                    return 6;
                case "Claim":
                    return 7;
                default:
                    return -1;
            }
        }

        private string GetApprovalNameFromTemplate(Axapta ax, string templateName)
        {
            for (AxaptaObject axaptaObject = (AxaptaObject)((AxaptaObject)ax.CallStaticClassMethod("TreeNode", "findNode", (object)string.Format("\\Workflow\\Workflow Templates\\{0}\\Required Elements", (object)templateName))).Call("AOTfirstChild"); axaptaObject != null; axaptaObject = (AxaptaObject)axaptaObject.Call("AOTnextSibling"))
            {
                if (axaptaObject.Call("AOTgetProperty", (object)"Type").ToString() == "Approval")
                    return axaptaObject.Call("AOTgetProperty", (object)"Name").ToString();
            }
            return string.Empty;
        }

        private Axapta GetAxInstance(
          string userName,
          string password,
          string domain,
          string company)
        {
            Axapta axapta = new Axapta();
            NetworkCredential networkCredential = new NetworkCredential(userName, password, domain);
            if (company == "Organization-Wide")
                company = string.Empty;
            axapta.LogonAs(userName, domain, networkCredential, company, ConfigurationManager.AppSettings["language"], ConfigurationManager.AppSettings["objectServer"], ConfigurationManager.AppSettings["configuration"]);
            return axapta;
        }

        private void GetBusinessJustification(
          Axapta ax,
          WorkItem workItem,
          string refTableId,
          string refRecId,
          string company)
        {
            AxaptaRecord axaptaRecord = ax.CreateAxaptaRecord("PurchReqBusJustification");
            string str = string.Format("select %1 where %1.RefTableId == {0} && %1.RefRecId == {1}", (object)refTableId, (object)refRecId, (object)company);
            ax.ExecuteStmt(str, axaptaRecord);
            if (!axaptaRecord.Found == true)
                return;
            workItem.Justification = (string)axaptaRecord.get_Field("BusinessJustification");
        }

        private string GetCommentFromTrackingId(Axapta ax, string trackingId)
        {
            string empty = string.Empty;
            AxaptaRecord axaptaRecord = ax.CreateAxaptaRecord("WorkflowTrackingCommentTable");
            string str = string.Format("Select %1 where %1.TrackingId == str2guid('{0}') ", (object)trackingId);
            ax.ExecuteStmt(str, axaptaRecord);
            if (axaptaRecord.Found == true)
                empty = axaptaRecord.get_Field("Comment").ToString();
            return empty;
        }

        private string GetCompanyName(Axapta ax, string dataAreaId)
        {
            AxaptaRecord axaptaRecord = ax.CreateAxaptaRecord("DataArea");
            string empty = string.Empty;
            string str = string.Format("select %1 where %1.IsVirtual == 0 && %1.Id == '{0}'", (object)dataAreaId);
            ax.ExecuteStmt(str, axaptaRecord);
            if (axaptaRecord.Found == true)
                empty = axaptaRecord.get_Field("Name").ToString();
            return empty;
        }

        private string GetDimensionDescription(Axapta ax, int dimensionCode, string num)
        {
            AxaptaRecord axaptaRecord = ax.CreateAxaptaRecord("Dimensions");
            string str = string.Format("Select %1 where %1.DimensionCode == {0} && %1.Num == '{1}'", (object)dimensionCode, (object)num);
            ax.ExecuteStmt(str, axaptaRecord);
            if (axaptaRecord.Found == true)
                return (string)axaptaRecord.get_Field("Description");
            return string.Empty;
        }

        private void GetDimensions(
          Axapta ax,
          AxaptaRecord tReqLine,
          RequisitionItem reqItem,
          string dataAreaIdField)
        {
            try
            {
                AxaptaObject axaptaObject = (AxaptaObject)ax.CallStaticClassMethod("DimensionAttributeValueSetStorage", "find", tReqLine.get_Field("DefaultDimension"));
                List<Dimension> dimensionList = new List<Dimension>();
                for (int index = 1; index <= (int)axaptaObject.Call("elements"); ++index)
                {
                    Dimension dimension = new Dimension();
                    dimension.DimensionLabel = ((AxaptaRecord)ax.CallStaticRecordMethod("DimensionAttribute", "find", axaptaObject.Call("getAttributeByIndex", (object)index))).get_Field("Name").ToString();
                    string str1 = axaptaObject.Call("getDisplayValueByIndex", (object)index).ToString();
                    if (!string.IsNullOrEmpty(str1))
                    {
                        dimension.DimensionValue = str1;
                    }
                    else
                    {
                        dimension.DimensionValue = string.Empty;
                        try
                        {
                            string str2 = ax.CallStaticClassMethod("OnTheGoHelper", "GetDimensionValue", axaptaObject.Call("getValueByIndex", (object)index), tReqLine.get_Field(dataAreaIdField)).ToString();
                            dimension.DimensionValue = str2;
                        }
                        catch
                        {
                        }
                    }
                    dimensionList.Add(dimension);
                }
                reqItem.Dimensions = dimensionList;
            }
            catch (Exception ex)
            {
                reqItem.Dimensions = new List<Dimension>()
        {
          new Dimension()
          {
            DimensionLabel = "Error",
            DimensionValue = ex.Message
          }
        };
            }
        }

        private string GetEmployeeId(Axapta ax, AxaptaRecord tWorkflowItem, string company)
        {
            string originator = this.GetOriginator(ax, tWorkflowItem);
            if (originator != string.Empty)
                return originator;
            return string.Empty;
        }

        private string GetEmployeeName(Axapta ax, string employeeId)
        {
            AxaptaRecord axaptaRecord1 = ax.CreateAxaptaRecord("DirPersonUser");
            string str1 = string.Format("select %1 where %1.User == '{0}'", (object)employeeId);
            ax.ExecuteStmt(str1, axaptaRecord1);
            if (axaptaRecord1.Found == true)
            {
                AxaptaRecord axaptaRecord2 = ax.CreateAxaptaRecord("DirPersonName");
                string str2 = string.Format("select %1 where %1.Person == {0}", (object)axaptaRecord1.get_Field("PersonParty").ToString());
                ax.ExecuteStmt(str2, axaptaRecord2);
                if (axaptaRecord1.Found == true)
                    return axaptaRecord2.get_Field("FirstName").ToString() + " " + axaptaRecord2.get_Field("LastName").ToString();
            }
            return employeeId;
        }

        private EnumEntity GetEnumById(List<EnumEntity> enumsList, int id)
        {
            foreach (EnumEntity enums in enumsList)
            {
                if (enums.EnumValue == id.ToString())
                    return enums;
            }
            return (EnumEntity)null;
        }

        private string GetEnumLabelByEnumValue(Axapta ax, string objectName, int enumValue)
        {
            AxaptaObject axaptaObject = (AxaptaObject)((AxaptaObject)ax.CallStaticClassMethod("TreeNode", "findNode", (object)string.Format("\\Data Dictionary\\Base Enums\\{0}", (object)objectName))).Call("AOTfirstChild");
            string str = string.Empty;
            for (; axaptaObject != null; axaptaObject = (AxaptaObject)axaptaObject.Call("AOTnextSibling"))
            {
                int int32 = Convert.ToInt32(axaptaObject.Call("AOTgetProperty", (object)"EnumValue"));
                if (enumValue == int32)
                {
                    str = this.GetLabelDescription(ax, axaptaObject.Call("AOTgetProperty", (object)"Label").ToString());
                    break;
                }
            }
            return str;
        }

        private string GetEnumTypeForStatus(Axapta ax, string tableName)
        {
            return ((AxaptaObject)ax.CallStaticClassMethod("TreeNode", "findNode", (object)string.Format("\\Data Dictionary\\Tables\\{0}\\Fields\\Status", (object)tableName))).Call("AOTgetProperty", (object)"EnumType").ToString();
        }

        private string GetFirstCompany(string companies)
        {
            if (companies != null && companies.Length > 0)
            {
                string[] strArray = companies.Split(',');
                if (strArray.Length > 0)
                {
                    for (int index = 0; index < strArray.Length; ++index)
                    {
                        if (strArray[index] != "-1")
                            return strArray[index];
                    }
                }
            }
            return string.Empty;
        }

        private List<HistoricalItem> GetHistoricalItems(
          Axapta ax,
          string correlationId)
        {
            List<HistoricalItem> historicalItemList = new List<HistoricalItem>();
            AxaptaRecord axaptaRecord1 = ax.CreateAxaptaRecord("WorkflowTrackingTable");
            AxaptaRecord axaptaRecord2 = ax.CreateAxaptaRecord("WorkflowTrackingStatusTable");
            string str1 = string.Format("Select * from %1 join %2 where %1.WorkflowTrackingStatusTable == %2.RecId && %2.CorrelationId == str2guid('{0}')", (object)correlationId);
            ax.ExecuteStmt(str1, axaptaRecord1, axaptaRecord2);
            while (axaptaRecord1.Found == true)
            {
                int int32_1 = Convert.ToInt32(axaptaRecord1.get_Field("TrackingContext"));
                int int32_2 = Convert.ToInt32(axaptaRecord1.get_Field("TrackingType"));
                if (int32_1 == 1 && int32_2 == 23)
                    historicalItemList.Add(new HistoricalItem()
                    {
                        Item = this.GetLabelDescription(ax, "@SYS108727").Replace("%1", this.GetEmployeeName(ax, axaptaRecord1.get_Field("User").ToString())),
                        Date = this.GetDateApplyTimezone(ax, Convert.ToDateTime(axaptaRecord1.get_Field("createdDateTime"))).ToString(),
                        Comment = this.GetCommentFromTrackingId(ax, axaptaRecord1.get_Field("TrackingId").ToString())
                    });
                if ((int32_1 == 4 || int32_1 == 5) && (int32_1 != 4 || int32_2 != 8))
                {
                    string empty = string.Empty;
                    string str2 = "     ";
                    HistoricalItem historicalItem = new HistoricalItem();
                    bool flag = true;
                    string str3;
                    if (int32_1 == 5 && int32_2 == 9)
                    {
                        str3 = str2 + this.GetLabelDescription(ax, "@SYS110789").Replace("%1", "") + (int32_1 == 4 ? string.Empty : " - " + this.GetEmployeeName(ax, axaptaRecord1.get_Field("User").ToString()));
                        historicalItem.Date = this.GetDateApplyTimezone(ax, Convert.ToDateTime(axaptaRecord1.get_Field("createdDateTime"))).ToString();
                    }
                    else if (int32_1 == 4 && int32_2 == 9)
                    {
                        str3 = this.GetLabelDescription(ax, "@SYS110788").Replace("%1", axaptaRecord1.get_Field("Name").ToString());
                    }
                    else
                    {
                        str3 = string.Format("{0}{1} - {2} - {3}<BR>{4}", (object)str2, (object)this.GetEnumLabelByEnumValue(ax, "WorkflowTrackingContext", int32_1), (object)this.GetEnumLabelByEnumValue(ax, "WorkflowTrackingType", int32_2), int32_1 == 4 ? (object)string.Empty : (object)this.GetEmployeeName(ax, axaptaRecord1.get_Field("User").ToString()), (object)"");
                        historicalItem.Date = this.GetDateApplyTimezone(ax, Convert.ToDateTime(axaptaRecord1.get_Field("createdDateTime"))).ToString();
                    }
                    if (flag)
                    {
                        historicalItem.Item = str3;
                        historicalItem.Comment = this.GetCommentFromTrackingId(ax, axaptaRecord1.get_Field("TrackingId").ToString());
                        historicalItem.Icon = this.GetIconFromTrackingType(int32_2);
                        historicalItemList.Add(historicalItem);
                    }
                }
                axaptaRecord1.Next();
            }
            return historicalItemList;
        }

        private int GetIconFromTrackingType(int trackingType)
        {
            int num;
            switch (trackingType)
            {
                case 4:
                case 8:
                    num = 1;
                    break;
                case 19:
                case 33:
                    num = 2;
                    break;
                default:
                    num = 0;
                    break;
            }
            return num;
        }

        private string GetItemDescription(
          Axapta ax,
          string itemId,
          string tableName,
          string itemFieldName,
          string descriptionFieldName)
        {
            AxaptaRecord axaptaRecord = ax.CreateAxaptaRecord(tableName);
            string str = string.Format("Select %1 where %1.{0} == \"{1}\"", (object)itemFieldName, (object)itemId);
            ax.ExecuteStmt(str, axaptaRecord);
            if (axaptaRecord.Found == true)
                return axaptaRecord.get_Field(descriptionFieldName).ToString();
            return string.Empty;
        }

        private string GetItemName(Axapta ax, int accountType, string accountNum)
        {
            switch (accountType)
            {
                case 0:
                    return this.GetItemDescription(ax, accountNum, "LedgerTable", "AccountNum", "AccountName");
                case 1:
                    return this.GetItemDescription(ax, accountNum, "CustTable", "AccountNum", "Name");
                case 2:
                    return this.GetItemDescription(ax, accountNum, "VendTable", "AccountNum", "Name");
                case 5:
                    return this.GetItemDescription(ax, accountNum, "AssetTable", "AssetId", "Name");
                case 6:
                    return this.GetItemDescription(ax, accountNum, "BankAccountTable", "AccountId", "Name");
                default:
                    return string.Empty;
            }
        }

        private string GetLabelDescription(Axapta ax, string label)
        {
            return ax.CallStaticClassMethod("SysLabel", "labelId2String", (object)label).ToString();
        }

        private string GetLabelFromActionType(Axapta ax, string actionType)
        {
            AxaptaObject axaptaObject = (AxaptaObject)ax.CallStaticClassMethod("TreeNode", "findNode", (object)string.Format("\\Data Dictionary\\Base Enums\\WorkflowWorkItemActionType\\{0}", (object)actionType));
            return (string)ax.CallStaticClassMethod("SysLabel", "labelId2String", axaptaObject.Call("AOTgetProperty", (object)"Label"));
        }

        private string GetLabelFromMenuItem(Axapta ax, string menuItem)
        {
            AxaptaObject axaptaObject = (AxaptaObject)ax.CallStaticClassMethod("TreeNode", "findNode", (object)string.Format("\\Menu Items\\Action\\{0}", (object)menuItem));
            string empty = string.Empty;
            if (axaptaObject != null)
            {
                try
                {
                    if (axaptaObject.Call("AOTgetProperty", (object)"Label") != null)
                    {
                        empty = axaptaObject.Call("AOTgetProperty", (object)"Label").ToString();
                        string str = (string)ax.CallStaticClassMethod("SysLabel", "labelId2String", (object)empty);
                        if (!string.IsNullOrEmpty(str))
                            return str;
                    }
                }
                catch
                {
                }
            }
            return empty;
        }

        private static string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] hash = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < hash.Length; ++index)
                stringBuilder.Append(hash[index].ToString("x2"));
            return stringBuilder.ToString();
        }

        private string GetOriginator(Axapta ax, AxaptaRecord tWorkflowItem)
        {
            object field = tWorkflowItem.get_Field("CorrelationId");
            ax.CreateAxaptaRecord("WorkflowTrackingStatusTable");
            object[] objArray = new object[1] { field };
            AxaptaRecord axaptaRecord = (AxaptaRecord)ax.CallStaticRecordMethod("WorkflowTrackingStatusTable", "findByCorrelation", objArray);
            if (axaptaRecord.Found == true)
                return axaptaRecord.get_Field("originator").ToString();
            return string.Empty;
        }

        private string GetOriginatorImage(Axapta ax, AxaptaRecord tWorkflowItem, string company)
        {
            try
            {
                string originator = this.GetOriginator(ax, tWorkflowItem);
                AxaptaRecord axaptaRecord1 = ax.CreateAxaptaRecord("DirPersonUser");
                AxaptaRecord axaptaRecord2 = ax.CreateAxaptaRecord("DirPartyTable");
                string str1 = string.Format("select * from %1 join %2 where %2.RECID == %1.PERSONPARTY && (%1.User == '{0}')", (object)originator);
                ax.ExecuteStmt(str1, axaptaRecord1, axaptaRecord2);
                if (axaptaRecord1.Found == true)
                {
                    string str2 = axaptaRecord1.get_Field("PersonParty").ToString();
                    object field = ((AxaptaRecord)ax.CallStaticRecordMethod("HcmPersonImage", "findByPerson", (object)str2, (object)true)).get_Field("Image");
                    if (field != null)
                    {
                        object[] objArray = new object[1] { field };
                        this.UnwrapImage(ax.CallStaticClassMethod("OnTheGoHelper", "GetBase64Str", objArray).ToString(), str2.ToString());
                        return string.Format("{0}.png", (object)str2);
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        private string GetSymbolForCurrency(Axapta ax, string currencyCode, string companies)
        {
            AxaptaRecord axaptaRecord = ax.CreateAxaptaRecord("Currency");
            string str = string.Format("select %1 where %1.CurrencyCode=='{0}'", (object)currencyCode, (object)companies);
            ax.ExecuteStmt(str, axaptaRecord);
            if (axaptaRecord.Found == true)
                return (string)axaptaRecord.get_Field("Symbol");
            return string.Empty;
        }

        private string GetTableLabel(Axapta ax, int tableNum)
        {
            return ((AxaptaObject)ax.CreateAxaptaObject("Dictionary").Call("tableObject", (object)tableNum)).Call("label").ToString();
        }

        private string GetTemplateName(Axapta ax, string configId)
        {
            AxaptaRecord axaptaRecord = ax.CreateAxaptaRecord("WorkflowConfigurationTable");
            string str = string.Format("Select %1 where %1.ConfigurationId == str2guid('{0}')", (object)configId);
            ax.ExecuteStmt(str, axaptaRecord);
            if (axaptaRecord.Found == true)
                return (string)axaptaRecord.get_Field("TemplateName");
            return string.Empty;
        }

        private string GetText(string s)
        {
            if (s.Split(':').Length <= 0)
                return s.Trim();
            return s.Split(':')[1].Trim();
        }

        private string GetUserNameComplete(
          string userName,
          string password,
          string domain,
          string utcTime,
          string signatureValue)
        {
            this.ValidateSignature(userName, password, utcTime, signatureValue);
            password = new SecurityHelper().Decrypt(password, true);
            Axapta axapta = (Axapta)null;
            List<DataArea> dataAreaList = new List<DataArea>();
            try
            {
                axapta = this.GetAxInstance(userName, password, domain, string.Empty);
                axapta.Logoff();
                axapta.Dispose();
                return string.Empty;
            }
            catch (Exception ex)
            {
                if (axapta != null)
                {
                    axapta.Logoff();
                    axapta.Dispose();
                }
                throw new Exception(string.Format("Error calling method GetUsername - {0} - {1}", (object)ex.Message, this.showStackTrace ? (object)ex.StackTrace : (object)string.Empty));
            }
        }

        private string GetUserNameFromPartyId(Axapta ax, string partyId)
        {
            object[] objArray = new object[1] { (object)partyId };
            AxaptaRecord axaptaRecord = (AxaptaRecord)ax.CallStaticRecordMethod("DirPartyTable", "find", objArray);
            if (axaptaRecord.Found == true)
                return axaptaRecord.get_Field("Name").ToString();
            return string.Empty;
        }

        private WorkItem GetWorkItemDet(
          Axapta ax,
          AxaptaRecord tWorkflowItem,
          object oRefTableId,
          object oRefRecId,
          object oRecId,
          AxaptaRecord tRelatedTable,
          string tableLabel,
          string tblName,
          string company)
        {
            WorkItem workItem = new WorkItem();
            workItem.Originator = string.Empty;
            workItem.DataAreaId = company;
            workItem.DataAreaName = this.GetCompanyName(ax, company);
            workItem.RecId = Convert.ToDouble(oRecId.ToString());
            workItem.Name = this.RemoveUnnecessaryCaption(tRelatedTable.Caption);
            workItem.Requisitioner = string.Empty;
            workItem.DateTime = this.GetDateApplyTimezone(ax, Convert.ToDateTime(tWorkflowItem.get_Field("DueDateTime"))).ToString();
            workItem.TableLabel = tableLabel;
            workItem.TableName = tblName;
            workItem.Subject = (string)tWorkflowItem.get_Field("Subject");
            workItem.Description = (string)tWorkflowItem.get_Field("Description");
            workItem.Status = string.Empty;
            workItem.Type = string.Empty;
            workItem.ImageURL = this.GetOriginatorImage(ax, tWorkflowItem, company);
            AxaptaRecord axaptaRecord = ax.CreateAxaptaRecord("WorkflowElementTable");
            string str = string.Format("Select %1 where %1.ElementId == str2guid('{0}')", tWorkflowItem.get_Field("ElementId"));
            ax.ExecuteStmt(str, axaptaRecord);
            workItem.ElementName = axaptaRecord.get_Field("ElementName").ToString();
            workItem.ElementType = Convert.ToInt32(axaptaRecord.get_Field("ElementType"));
            bool boolean = Convert.ToBoolean(tWorkflowItem.get_Field("IsClaimed"));
            workItem.Actions = this.GetActions(ax, boolean, workItem.ElementName, workItem.ElementType);
            this.GetWorkItemItems(ax, tWorkflowItem, oRefTableId, oRefRecId, tRelatedTable, tblName, workItem, company);
            workItem.HistoricalItems = this.GetHistoricalItems(ax, tWorkflowItem.get_Field("CorrelationId").ToString());
            this.VerifyAttachments(ax, tWorkflowItem, oRefTableId, oRefRecId, workItem);
            return workItem;
        }

        private void GetWorkItemItems(
          Axapta ax,
          AxaptaRecord tWorkflowItem,
          object oRefTableId,
          object oRefRecId,
          AxaptaRecord tRelatedTable,
          string tblName,
          WorkItem workItem,
          string company)
        {
            workItem._Items = new List<RequisitionItem>();
            workItem.Requisitioner = this.GetEmployeeName(ax, this.GetEmployeeId(ax, tWorkflowItem, company));
            switch (tblName)
            {
                case "PurchReqTable":
                    workItem.Type = tRelatedTable.get_Field("PurchReqType").ToString();
                    this.GetPurchReqInfo(ax, workItem, oRefTableId.ToString(), oRefRecId.ToString(), company);
                    break;
                case "PurchTable":
                    this.GetPurchInfo(ax, workItem, oRefTableId.ToString(), oRefRecId.ToString(), company);
                    break;
                case "SalesTable":
                    this.GetSalesInfo(ax, workItem, oRefTableId.ToString(), oRefRecId.ToString(), company);
                    break;
                case "LedgerJournalTable":
                    this.GetLedgerTableInfo(ax, workItem, oRefTableId.ToString(), oRefRecId.ToString(), company);
                    break;
                case "TrvExpTable":
                    this.GetTrvExp(ax, workItem, oRefTableId.ToString(), oRefRecId.ToString(), company);
                    break;
                case "TrvExpTrans":
                    this.GetTrvExpTrans(ax, workItem, oRefTableId.ToString(), oRefRecId.ToString(), company);
                    break;
                case "TrvCashAdvance":
                    this.GetTrvCashAdvance(ax, workItem, oRefTableId.ToString(), oRefRecId.ToString(), company);
                    break;
                case "TrvRequisitionTable":
                    this.GetTrvRequisition(ax, workItem, oRefTableId.ToString(), oRefRecId.ToString(), company);
                    break;
                case "TSTimesheetTable":
                    this.GetTSTimeSheet(ax, workItem, oRefTableId.ToString(), oRefRecId.ToString(), company);
                    break;
            }
            if (!(tblName.ToLower() == "salesquotationtable"))
                return;
            this.GetSalesQuotation(ax, workItem, oRefTableId.ToString(), oRefRecId.ToString(), company);
        }

        private void GetSalesQuotation(
          Axapta ax,
          WorkItem workItem,
          string refTableId,
          string refRecId,
          string company)
        {
            AxaptaRecord axaptaRecord1 = ax.CreateAxaptaRecord("SalesQuotationTable");
            AxaptaRecord axaptaRecord2 = ax.CreateAxaptaRecord("SalesQuotationLine");
            string str1 = string.Format("select * from %1 join %2 where %2.QuotationId == %1.QuotationId && (%1.RecId == {0})", (object)refRecId, (object)company);
            ax.ExecuteStmt(str1, axaptaRecord1, axaptaRecord2);
            Decimal num1 = new Decimal(0);
            while (axaptaRecord1.Found == true)
            {
                RequisitionItem requisitionItem1 = new RequisitionItem();
                requisitionItem1.UnitPrice = Convert.ToDecimal(axaptaRecord2.get_Field("SalesPrice")).ToString("N2");
                requisitionItem1.Qty = Convert.ToDecimal(axaptaRecord2.get_Field("SalesQty"));
                try
                {
                    requisitionItem1.Item = axaptaRecord2.Call("itemName").ToString();
                }
                catch (Exception ex)
                {
                    requisitionItem1.Item = ex.Message;
                }
                requisitionItem1.CurrencyCode = this.GetSymbolForCurrency(ax, axaptaRecord2.get_Field("CurrencyCode").ToString(), company);
                RequisitionItem requisitionItem2 = requisitionItem1;
                Decimal num2 = Convert.ToDecimal(axaptaRecord2.get_Field("SalesPrice"));
                string str2 = num2.ToString("N2");
                requisitionItem2.LineAmount = str2;
                requisitionItem1.Dimensions = new List<Dimension>();
                try
                {
                    string batchId = this.GetBatchId(ax, axaptaRecord2);
                    requisitionItem1.Dimensions.Add(new Dimension()
                    {
                        DimensionLabel = "Num FDX",
                        DimensionValue = batchId
                    });
                    requisitionItem1.Dimensions.Add(new Dimension()
                    {
                        DimensionLabel = "Age FDX",
                        DimensionValue = this.GetProdDate(ax, axaptaRecord2, batchId)
                    });
                    Dimension dimension1 = new Dimension();
                    dimension1.DimensionLabel = "MT Remise";
                    Dimension dimension2 = dimension1;
                    num2 = Convert.ToDecimal(axaptaRecord2.get_Field("LineDisc"));
                    string str3 = num2.ToString("N2");
                    dimension2.DimensionValue = str3;
                    requisitionItem1.Dimensions.Add(dimension1);
                    requisitionItem1.Dimensions.Add(new Dimension()
                    {
                        DimensionLabel = "% Remise",
                        DimensionValue = Convert.ToDecimal(axaptaRecord2.get_Field("LinePercent")).ToString("N2")
                    });
                }
                catch (Exception ex)
                {
                    requisitionItem1.Dimensions.Add(new Dimension()
                    {
                        DimensionLabel = "Err",
                        DimensionValue = ex.Message
                    });
                }
                num1 += Convert.ToDecimal(axaptaRecord2.get_Field("LineAmount"));
                workItem._Items.Add(requisitionItem1);
                axaptaRecord1.Next();
            }
            workItem.TotalValue = num1;
            workItem.TotalValueDisplay = workItem.TotalValue.ToString("N2");
        }

        private string GetProdDate(Axapta ax, AxaptaRecord tLine, string inventBatchId)
        {
            AxaptaRecord axaptaRecord = ax.CreateAxaptaRecord("InventBatch");
            string str = string.Format("select * from %1 where (%1.InventBatchId == '{0}')", (object)inventBatchId);
            ax.ExecuteStmt(str, axaptaRecord);
            if (axaptaRecord.Found == true)
            {
                object field = axaptaRecord.get_Field("ProdDate");
                if (field != null)
                    return Convert.ToDateTime(field).ToString("dd/MM/yyyy");
            }
            return string.Empty;
        }

        private string GetBatchId(Axapta ax, AxaptaRecord tLine)
        {
            AxaptaRecord axaptaRecord = ax.CreateAxaptaRecord("InventDim");
            if (tLine.get_Field("InventDimId") != null)
            {
                string str = string.Format("select * from %1 where (%1.InventDimId == '{0}')", tLine.get_Field("InventDimId"));
                ax.ExecuteStmt(str, axaptaRecord);
                if (axaptaRecord.Found == true)
                    return axaptaRecord.get_Field("InventBatchId").ToString();
            }
            return string.Empty;
        }

        private void GetWorkItemTotalValue(
          Axapta ax,
          AxaptaRecord tWorkflowItem,
          object oRefTableId,
          object oRefRecId,
          AxaptaRecord tRelatedTable,
          string tblName,
          WorkItem workItem,
          string company)
        {
            switch (tblName)
            {
                case "PurchReqTable":
                    this.GetPurchReqInfoTotalValue(ax, workItem, oRefTableId.ToString(), oRefRecId.ToString(), company);
                    this.GetBusinessJustification(ax, workItem, oRefTableId.ToString(), oRefRecId.ToString(), company);
                    break;
                case "PurchTable":
                    this.GetPurchInfoTotalValue(ax, workItem, oRefTableId.ToString(), oRefRecId.ToString(), company);
                    break;
                case "SalesTable":
                    this.GetSalesInfoTotalValue(ax, workItem, oRefTableId.ToString(), oRefRecId.ToString(), company);
                    break;
                case "LedgerJournalTable":
                    this.GetLedgerTableInfoTotalValue(ax, workItem, oRefTableId.ToString(), oRefRecId.ToString(), company);
                    break;
                case "TrvExpTable":
                    this.GetTrvExpTotalValue(ax, workItem, oRefTableId.ToString(), oRefRecId.ToString(), company);
                    workItem.Justification = tRelatedTable.get_Field("Txt1").ToString();
                    break;
                case "TrvExpTrans":
                    this.GetTrvExpTransTotalValue(ax, workItem, oRefTableId.ToString(), oRefRecId.ToString(), company);
                    break;
                case "TrvRequisitionTable":
                    this.GetTrvRequisitionTableTotalValue(ax, workItem, oRefTableId.ToString(), oRefRecId.ToString(), company);
                    break;
                case "TrvCashAdvance":
                    this.GetTrvCashAdvanceTotalValue(ax, workItem, oRefTableId.ToString(), oRefRecId.ToString(), company);
                    workItem.Justification = tRelatedTable.get_Field("Notes").ToString();
                    break;
                case "TSTimesheetTable":
                    this.GetTSTimeSheetTotalValue(ax, workItem, oRefTableId.ToString(), oRefRecId.ToString(), company);
                    break;
            }
        }

        private bool HistoricalWorkItemCreateLine(
          Axapta ax,
          string correlationId,
          string recId,
          string currentUser)
        {
            bool flag = true;
            AxaptaRecord axaptaRecord = ax.CreateAxaptaRecord("WorkflowTrackingTable");
            string str = string.Format("Select %1 order by %1.RecId where %1.CorrelationId == str2guid('{0}') && %1.RecId > {1}", (object)correlationId, (object)recId);
            ax.ExecuteStmt(str, axaptaRecord);
            while (axaptaRecord.Found == true && Convert.ToInt32(axaptaRecord.get_Field("TrackingContext")) == 5)
            {
                if (axaptaRecord.get_Field("User").ToString() == currentUser)
                    flag = false;
                axaptaRecord.Next();
            }
            return flag;
        }

        private bool IsDataAreaIncluded(string dataAreaId, string companies)
        {
            string str1 = companies;
            char[] chArray = new char[1] { ',' };
            foreach (string str2 in str1.Split(chArray))
            {
                if (str2.ToUpper().Trim() == dataAreaId.ToUpper().Trim() || str2 == "-1" && dataAreaId == string.Empty)
                    return true;
            }
            return false;
        }

        private bool IsUserAllowed(string userName)
        {
            SqlDatabase sqlDatabase = new SqlDatabase(this.ConnectionStringAdmin);
            DbCommand sqlStringCommand = ((Database)sqlDatabase).GetSqlStringCommand("  select * \r\n                                from PortalMember pm\t\t                             \r\n                               WHERE NetworkUser = @userName");
            ((Database)sqlDatabase).AddInParameter(sqlStringCommand, nameof(userName), DbType.String, (object)userName);
            return ((Database)sqlDatabase).ExecuteDataSet(sqlStringCommand).Tables[0].Rows.Count > 0;
        }

        private string RemoveUnnecessaryCaption(string caption)
        {
            return caption.Replace(", número do diário", string.Empty).Replace(", journal number", string.Empty);
        }

        private void UnwrapImage(string s, string recId)
        {
            Bitmap bitmap = this.ResizeBitmap((Bitmap)Image.FromStream((Stream)new MemoryStream(Convert.FromBase64String(s))), 50, 50);
            string str = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", string.Empty).Replace("\\bin", string.Empty);
            bitmap.SetResolution(70f, 70f);
            bitmap.Save(string.Format("{0}\\services\\Images\\{1}.png", (object)str, (object)recId));
        }

        private void UpdateDataArea(DataSet ds)
        {
            DataSet dataSet = new DataSet();
            using (SqlConnection connection = new SqlConnection(this.ConnectionStringAdmin))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand("select * from PortalMemberDataArea", connection);
                SqlCommandBuilder sqlCommandBuilder = new SqlCommandBuilder(adapter);
                adapter.UpdateCommand = sqlCommandBuilder.GetUpdateCommand();
                adapter.Update(ds);
            }
        }

        private void UpdateMember(DataSet ds)
        {
            DataSet dataSet = new DataSet();
            using (SqlConnection connection = new SqlConnection(this.ConnectionStringAdmin))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand("select * from PortalMember", connection);
                SqlCommandBuilder sqlCommandBuilder = new SqlCommandBuilder(adapter);
                adapter.UpdateCommand = sqlCommandBuilder.GetUpdateCommand();
                adapter.Update(ds);
            }
        }

        private bool ValidateHash(DataSet ds)
        {
            DataRow row = ds.Tables["Account"].Rows[0];
            if (row["AccountType"] != DBNull.Value)
            {
                SecurityHelper securityHelper = new SecurityHelper();
                string hashForString = securityHelper.CreateHashForString(row["SerialNumber"].ToString() + row["N"].ToString() + row["D"].ToString() + row["U"].ToString());
                DateTime result1 = DateTime.MinValue;
                DateTime.TryParse(securityHelper.Decrypt(row["D"].ToString(), true), out result1);
                int result2 = 0;
                int.TryParse(securityHelper.Decrypt(row["N"].ToString(), true), out result2);
                if (hashForString != row["Hash"].ToString() || result1 < DateTime.Now || result2 < ds.Tables["Members"].Rows.Count)
                    return false;
            }
            return true;
        }

        private void ValidateSignature(
          string userName,
          string password,
          string currentUTCTime,
          string signature)
        {
            string input = userName + password + currentUTCTime + "2B05456F-7B1C-482D-8FB1-1F50350C31DA";
            string md5Hash1;
            using (MD5 md5Hash2 = MD5.Create())
                md5Hash1 = GetMd5Hash(md5Hash2, input);
            string base64String = Convert.ToBase64String(new UTF8Encoding().GetBytes(md5Hash1));
            if (signature != base64String)
                throw new Exception("Invalid signature.");
        }

        private void VerifyAttachments(
          Axapta ax,
          AxaptaRecord tWorkflowItem,
          object oRefTableId,
          object oRefRecId,
          WorkItem workItem)
        {
            AxaptaRecord axaptaRecord = ax.CreateAxaptaRecord("DocuRef");
            string str = string.Format("Select %1 where %1.RefTableId == {0} && %1.RefRecId == {1}", oRefTableId, oRefRecId);
            ax.ExecuteStmt(str, axaptaRecord);
            int num = 0;
            while (axaptaRecord.Found == true)
            {
                ++num;
                axaptaRecord.Next();
            }
            workItem.AttachmentsCount = num;
        }

        private WorkItem GetWorkItem(
          Axapta ax,
          AxaptaRecord tWorkflowItem,
          object oRefTableId,
          object oRefRecId,
          object oRecId,
          AxaptaRecord tRelatedTable,
          string tableLabel,
          string tblName,
          string company,
          bool includeItems)
        {
            WorkItem workItem = new WorkItem();
            workItem.Originator = string.Empty;
            workItem.DataAreaId = company == string.Empty ? "Organization-Wide" : company;
            workItem.DataAreaName = this.GetCompanyName(ax, company);
            workItem.RecId = Convert.ToDouble(oRecId.ToString());
            workItem.Name = this.RemoveUnnecessaryCaption(tRelatedTable.Caption);
            workItem.Requisitioner = this.GetEmployeeName(ax, this.GetEmployeeId(ax, tWorkflowItem, company));
            workItem.DateTime = this.GetDateApplyTimezone(ax, Convert.ToDateTime(tWorkflowItem.get_Field("DueDateTime"))).ToString();
            workItem.TableLabel = tableLabel;
            workItem.TableName = tblName;
            workItem.TemplateName = "";
            workItem.Subject = (string)tWorkflowItem.get_Field("Subject");
            workItem.Description = (string)tWorkflowItem.get_Field("Description");
            workItem.Status = string.Empty;
            workItem.Type = string.Empty;
            workItem.ImageURL = this.GetOriginatorImage(ax, tWorkflowItem, company);
            if (tblName == "PurchTable" || tblName == "SalesTable" || tblName == "LedgerJournalTable")
                workItem.CurrencyCode = this.GetSymbolForCurrency(ax, tRelatedTable.get_Field("CurrencyCode").ToString(), company);
            AxaptaRecord axaptaRecord = ax.CreateAxaptaRecord("WorkflowElementTable");
            string str = string.Format("Select %1 where %1.ElementId == str2guid('{0}')", tWorkflowItem.get_Field("ElementId"));
            ax.ExecuteStmt(str, axaptaRecord);
            workItem.ElementName = axaptaRecord.get_Field("ElementName").ToString();
            workItem.ElementType = Convert.ToInt32(axaptaRecord.get_Field("ElementType"));
            bool boolean = Convert.ToBoolean(tWorkflowItem.get_Field("IsClaimed"));
            workItem.Actions = this.GetActions(ax, boolean, workItem.ElementName, workItem.ElementType);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (DynamicsAppSolutionWebApi.Models.Action action in workItem.Actions)
                stringBuilder.AppendFormat("{0}, ", (object)action.ActionName);
            if (stringBuilder.Length > 0)
                stringBuilder.Remove(stringBuilder.Length - 2, 2);
            workItem.ActionsString = stringBuilder.ToString();
            if (includeItems)
            {
                this.GetWorkItemItems(ax, tWorkflowItem, oRefTableId, oRefRecId, tRelatedTable, tblName, workItem, company);
                workItem.HistoricalItems = this.GetHistoricalItems(ax, tWorkflowItem.get_Field("CorrelationId").ToString());
            }
            this.GetWorkItemTotalValue(ax, tWorkflowItem, oRefTableId, oRefRecId, tRelatedTable, tblName, workItem, company);
            this.VerifyAttachments(ax, tWorkflowItem, oRefTableId, oRefRecId, workItem);
            return workItem;
        }

        private void GetPurchReqInfo(
          Axapta ax,
          WorkItem workItem,
          string refTableId,
          string refRecId,
          string company)
        {
            this.GetBusinessJustification(ax, workItem, refTableId, refRecId, company);
            AxaptaRecord axaptaRecord1 = ax.CreateAxaptaRecord("PurchReqTable");
            AxaptaRecord axaptaRecord2 = ax.CreateAxaptaRecord("PurchReqLine");
            string str1 = string.Format("select * from %1 join %2 where %2.PurchReqTable == %1.RecId && (%1.RecId == {0})", (object)refRecId, (object)company);
            ax.ExecuteStmt(str1, axaptaRecord1, axaptaRecord2);
            string empty = string.Empty;
            Decimal num = new Decimal(0);
            string str2 = string.Empty;
            while (axaptaRecord1.Found == true)
            {
                workItem.DataAreaId = axaptaRecord2.get_Field("ItemIdDataArea").ToString();
                str2 = this.GetSymbolForCurrency(ax, axaptaRecord2.get_Field("CurrencyCode").ToString(), company);
                RequisitionItem reqItem = new RequisitionItem();
                reqItem.UnitPrice = Convert.ToDecimal(axaptaRecord2.get_Field("PurchPrice")).ToString("N2");
                reqItem.Qty = Convert.ToDecimal(axaptaRecord2.get_Field("PurchQty"));
                reqItem.Item = (string)axaptaRecord2.get_Field("ItemId") + " - " + axaptaRecord2.Call("editItemName").ToString();
                reqItem.CurrencyCode = str2;
                reqItem.LineAmount = Convert.ToDecimal(axaptaRecord2.get_Field("LineAmount")).ToString("N2");
                this.GetDimensions(ax, axaptaRecord2, reqItem, "ItemIdDataArea");
                reqItem.Dimensions.Insert(0, new Dimension()
                {
                    DimensionLabel = "Text",
                    DimensionValue = axaptaRecord2.get_Field("Name").ToString()
                });
                this.GetProjectInfo(ax, axaptaRecord2, reqItem);
                num += Convert.ToDecimal(axaptaRecord2.get_Field("LineAmount"));
                workItem._Items.Add(reqItem);
                axaptaRecord1.Next();
            }
            workItem.CurrencyCode = str2;
            workItem.TotalValue = num;
            workItem.TotalValueDisplay = workItem.TotalValue.ToString("N2");
        }

        private void GetProjectInfo(Axapta ax, AxaptaRecord tReqLine, RequisitionItem reqItem)
        {
            string empty1 = string.Empty;
            string empty2 = string.Empty;
            AxaptaRecord axaptaRecord1 = ax.CreateAxaptaRecord("ProjCategory");
            string str1 = string.Format("select * from %1 where %1.CategoryId == '{0}'", tReqLine.get_Field("ProjCategoryId"));
            ax.ExecuteStmt(str1, axaptaRecord1);
            if (axaptaRecord1.Found == true)
                empty1 = axaptaRecord1.get_Field("Name").ToString();
            AxaptaRecord axaptaRecord2 = ax.CreateAxaptaRecord("ProjTable");
            string str2 = string.Format("select * from %1 where %1.ProjId == '{0}'", tReqLine.get_Field("ProjId"));
            ax.ExecuteStmt(str2, axaptaRecord2);
            if (axaptaRecord2.Found == true)
                empty2 = axaptaRecord2.get_Field("Name").ToString();
            reqItem.Dimensions.Insert(1, new Dimension()
            {
                DimensionLabel = "Project ID",
                DimensionValue = string.Format("{0} - {1}", (object)tReqLine.get_Field("ProjId").ToString(), (object)empty2)
            });
            reqItem.Dimensions.Insert(2, new Dimension()
            {
                DimensionLabel = "Proj. Categ.",
                DimensionValue = string.Format("{0} - {1}", (object)tReqLine.get_Field("ProjCategoryId").ToString(), (object)empty1)
            });
        }

        private void GetPurchInfo(
          Axapta ax,
          WorkItem workItem,
          string refTableId,
          string refRecId,
          string company)
        {
            AxaptaRecord axaptaRecord1 = ax.CreateAxaptaRecord("PurchTable");
            AxaptaRecord axaptaRecord2 = ax.CreateAxaptaRecord("PurchLine");
            string str1 = string.Format("select * from %1 join %2 where %2.PurchId == %1.PurchId && (%1.RecId == {0})", (object)refRecId, (object)company);
            ax.ExecuteStmt(str1, axaptaRecord1, axaptaRecord2);
            string empty = string.Empty;
            Decimal num = new Decimal(0);
            Decimal totalValue;
            while (axaptaRecord1.Found == true)
            {
                RequisitionItem reqItem = new RequisitionItem();
                RequisitionItem requisitionItem1 = reqItem;
                totalValue = Convert.ToDecimal(axaptaRecord2.get_Field("PurchPrice"));
                string str2 = totalValue.ToString("N2");
                requisitionItem1.UnitPrice = str2;
                reqItem.Qty = Convert.ToDecimal(axaptaRecord2.get_Field("PurchQty"));
                reqItem.Item = axaptaRecord2.Call("ItemName").ToString();
                reqItem.CurrencyCode = this.GetSymbolForCurrency(ax, axaptaRecord2.get_Field("CurrencyCode").ToString(), company);
                RequisitionItem requisitionItem2 = reqItem;
                totalValue = Convert.ToDecimal(axaptaRecord2.get_Field("LineAmount"));
                string str3 = totalValue.ToString("N2");
                requisitionItem2.LineAmount = str3;
                this.GetDimensions(ax, axaptaRecord2, reqItem, "DataAreaId");
                Dimension dimension1 = new Dimension();
                dimension1.DimensionLabel = "PU HT";
                Dimension dimension2 = dimension1;
                totalValue = Convert.ToDecimal(axaptaRecord2.get_Field("PurchPrice"));
                string str4 = totalValue.ToString("N2");
                dimension2.DimensionValue = str4;
                reqItem.Dimensions.Add(dimension1);
                reqItem.Dimensions.Add(new Dimension()
                {
                    DimensionLabel = "Num Article",
                    DimensionValue = (string)axaptaRecord2.get_Field("ItemId")
                });
                num += Convert.ToDecimal(axaptaRecord2.get_Field("LineAmount"));
                workItem._Items.Add(reqItem);
                axaptaRecord1.Next();
            }
            workItem.TotalValue = num;
            WorkItem workItem1 = workItem;
            totalValue = workItem.TotalValue;
            string str5 = totalValue.ToString("N2");
            workItem1.TotalValueDisplay = str5;
        }

        private void GetPurchInfoTotalValue(
          Axapta ax,
          WorkItem workItem,
          string refTableId,
          string refRecId,
          string company)
        {
            AxaptaRecord axaptaRecord1 = ax.CreateAxaptaRecord("PurchTable");
            AxaptaRecord axaptaRecord2 = ax.CreateAxaptaRecord("PurchLine");
            string str = !string.IsNullOrEmpty(company) ? string.Format("container c = ['{1}']; select crosscompany: c * from %1 join %2 where %2.PurchId == %1.PurchId && (%1.RecId == {0})", (object)refRecId, (object)company) : string.Format("select %1 join %2 where %2.PurchId == %1.PurchId && (%1.RecId == {0})", (object)refRecId);
            ax.ExecuteStmt(str, axaptaRecord1, axaptaRecord2);
            Decimal num = new Decimal(0);
            while (axaptaRecord1.Found == true)
            {
                num += Convert.ToDecimal(axaptaRecord2.get_Field("LineAmount"));
                axaptaRecord1.Next();
            }
            workItem.TotalValue = num;
            workItem.TotalValueDisplay = workItem.TotalValue.ToString("N2");
        }

        private void GetPurchReqInfoTotalValue(
          Axapta ax,
          WorkItem workItem,
          string refTableId,
          string refRecId,
          string company)
        {
            AxaptaRecord axaptaRecord1 = ax.CreateAxaptaRecord("PurchReqTable");
            AxaptaRecord axaptaRecord2 = ax.CreateAxaptaRecord("PurchReqLine");
            string str = !string.IsNullOrEmpty(company) ? string.Format("container c = ['{1}']; select crosscompany: c * from %1 join %2 where %2.PurchReqTable == %1.RecId && (%1.RecId == {0})", (object)refRecId, (object)company) : string.Format("select * from %1 join %2 where %2.PurchReqTable == %1.RecId && (%1.RecId == {0})", (object)refRecId);
            ax.ExecuteStmt(str, axaptaRecord1, axaptaRecord2);
            string empty = string.Empty;
            Decimal num = new Decimal(0);
            while (axaptaRecord1.Found == true)
            {
                num += Convert.ToDecimal(axaptaRecord2.get_Field("LineAmount"));
                axaptaRecord1.Next();
            }
            workItem.TotalValue = num;
            workItem.TotalValueDisplay = workItem.TotalValue.ToString("N2");
        }

        private void GetSalesInfo(
          Axapta ax,
          WorkItem workItem,
          string refTableId,
          string refRecId,
          string company)
        {
            AxaptaRecord axaptaRecord1 = ax.CreateAxaptaRecord("SalesTable");
            AxaptaRecord axaptaRecord2 = ax.CreateAxaptaRecord("SalesLine");
            string str1 = string.Format("select * from %1 join %2 where %2.SalesId == %1.SalesId && (%1.RecId == {0})", (object)refRecId, (object)company);
            ax.ExecuteStmt(str1, axaptaRecord1, axaptaRecord2);
            string empty = string.Empty;
            Decimal num = new Decimal(0);
            Decimal totalValue;
            while (axaptaRecord1.Found == true)
            {
                RequisitionItem reqItem = new RequisitionItem();
                RequisitionItem requisitionItem1 = reqItem;
                totalValue = Convert.ToDecimal(axaptaRecord2.get_Field("SalesPrice"));
                string str2 = totalValue.ToString("N2");
                requisitionItem1.UnitPrice = str2;
                reqItem.Qty = new Decimal(1);
                reqItem.Item = (string)axaptaRecord2.get_Field("ItemId");
                reqItem.CurrencyCode = this.GetSymbolForCurrency(ax, axaptaRecord2.get_Field("CurrencyCode").ToString(), company);
                RequisitionItem requisitionItem2 = reqItem;
                totalValue = Convert.ToDecimal(axaptaRecord2.get_Field("LineAmount"));
                string str3 = totalValue.ToString("N2");
                requisitionItem2.LineAmount = str3;
                this.GetDimensions(ax, axaptaRecord2, reqItem, "DataAreaId");
                num += Convert.ToDecimal(axaptaRecord2.get_Field("LineAmount"));
                workItem._Items.Add(reqItem);
                axaptaRecord1.Next();
            }
            workItem.TotalValue = num;
            WorkItem workItem1 = workItem;
            totalValue = workItem.TotalValue;
            string str4 = totalValue.ToString("N2");
            workItem1.TotalValueDisplay = str4;
        }

        private void GetSalesInfoTotalValue(
          Axapta ax,
          WorkItem workItem,
          string refTableId,
          string refRecId,
          string company)
        {
            AxaptaRecord axaptaRecord1 = ax.CreateAxaptaRecord("SalesTable");
            AxaptaRecord axaptaRecord2 = ax.CreateAxaptaRecord("SalesLine");
            string str = !string.IsNullOrEmpty(company) ? string.Format("container c = ['{1}']; select crosscompany: c * from  %1 join %2 where %2.SalesId == %1.SalesId && (%1.RecId == {0})", (object)refRecId, (object)company) : string.Format("select * from  %1 join %2 where %2.SalesId == %1.SalesId && (%1.RecId == {0})", (object)refRecId);
            ax.ExecuteStmt(str, axaptaRecord1, axaptaRecord2);
            Decimal num = new Decimal(0);
            while (axaptaRecord1.Found == true)
            {
                num += Convert.ToDecimal(axaptaRecord2.get_Field("LineAmount"));
                axaptaRecord1.Next();
            }
            workItem.TotalValue = num;
            workItem.TotalValueDisplay = workItem.TotalValue.ToString("N2");
        }

        private Decimal GetTotalValueTrvExp(Axapta ax, string expNumber)
        {
            AxaptaRecord axaptaRecord = ax.CreateAxaptaRecord("TrvExpTrans");
            string str1 = string.Format("select firstonly sum(AmountMST) from %1 where\r\n\t                %1.ExpNumber == '{0}' &&\r\n\t                %1.ExpType != TrvExpType::Advance &&\r\n\t                %1.ExpType != TrvExpType::Personal &&\r\n\t                %1.LineType != TrvExpLineType::ItemizedHeader\r\n                ", (object)expNumber);
            ax.ExecuteStmt(str1, axaptaRecord);
            Decimal num1 = Convert.ToDecimal(axaptaRecord.get_Field("AmountMST"));
            string str2 = string.Format("select firstonly sum(AmountMST) from %1 where\r\n\t                    %1.ExpNumber == '{0}' &&\r\n\t                    %1.ExpType == TrvExpType::Advance\r\n                ", (object)expNumber);
            ax.ExecuteStmt(str2, axaptaRecord);
            Decimal num2 = Convert.ToDecimal(axaptaRecord.get_Field("AmountMST"));
            return num1 - num2;
        }

        private void GetTrvExpTransTotalValue(
          Axapta ax,
          WorkItem workItem,
          string refTableId,
          string refRecId,
          string company)
        {
            AxaptaRecord axaptaRecord = ax.CreateAxaptaRecord("TrvExpTrans");
            string str = !string.IsNullOrEmpty(company) ? string.Format("container c = ['{1}']; select crosscompany: c * from %1 where (%1.RecId == {0})", (object)refRecId, (object)company) : string.Format("select * from %1 where (%1.RecId == {0})", (object)refRecId);
            ax.ExecuteStmt(str, axaptaRecord);
            if (!axaptaRecord.Found == true)
                return;
            try
            {
                workItem.TotalValue = Convert.ToDecimal(axaptaRecord.get_Field("AmountMST"));
                workItem.TotalValueDisplay = workItem.TotalValue.ToString("N2");
                workItem.CurrencyCode = string.Empty;
            }
            catch (Exception ex)
            {
                WorkItem workItem1 = workItem;
                workItem1.Description = workItem1.Description + "err:" + ex.Message;
            }
        }

        private void GetTrvCashAdvanceTotalValue(
          Axapta ax,
          WorkItem workItem,
          string refTableId,
          string refRecId,
          string company)
        {
            AxaptaRecord axaptaRecord = ax.CreateAxaptaRecord("TrvCashAdvance");
            string str = string.Format("select * from %1 where (%1.RecId == {0})", (object)refRecId, (object)company);
            ax.ExecuteStmt(str, axaptaRecord);
            if (!axaptaRecord.Found == true)
                return;
            try
            {
                workItem.TotalValue = Convert.ToDecimal(axaptaRecord.get_Field("RequestedAmountCur"));
                workItem.TotalValueDisplay = workItem.TotalValue.ToString("N2");
            }
            catch (Exception ex)
            {
                WorkItem workItem1 = workItem;
                workItem1.Description = workItem1.Description + "err:" + ex.Message;
            }
        }

        private void GetTrvExpTotalValue(
          Axapta ax,
          WorkItem workItem,
          string refTableId,
          string refRecId,
          string company)
        {
            AxaptaRecord axaptaRecord1 = ax.CreateAxaptaRecord("TrvExpTable");
            AxaptaRecord axaptaRecord2 = ax.CreateAxaptaRecord("TrvExpTrans");
            string str = string.Format("select * from %1 join %2 where %2.ExpNumber == %1.ExpNumber && (%1.RecId == {0})", (object)refRecId, (object)company);
            ax.ExecuteStmt(str, axaptaRecord1, axaptaRecord2);
            if (!axaptaRecord1.Found == true)
                return;
            try
            {
                workItem.TotalValue = this.GetTotalValueTrvExp(ax, axaptaRecord1.get_Field("ExpNumber").ToString());
                workItem.TotalValueDisplay = workItem.TotalValue.ToString("N2");
                workItem.CurrencyCode = string.Empty;
            }
            catch (Exception ex)
            {
                WorkItem workItem1 = workItem;
                workItem1.Description = workItem1.Description + "err:" + ex.Message;
            }
        }

        private void GetTrvExp(
          Axapta ax,
          WorkItem workItem,
          string refTableId,
          string refRecId,
          string company)
        {
            AxaptaRecord axaptaRecord1 = ax.CreateAxaptaRecord("TrvExpTable");
            string str1 = string.Format("select * from %1 where (%1.RecId == {0})", (object)refRecId, (object)company);
            ax.ExecuteStmt(str1, axaptaRecord1);
            Decimal num1 = new Decimal(0);
            if (axaptaRecord1.Found == true)
            {
                try
                {
                    workItem.TotalValue = this.GetTotalValueTrvExp(ax, axaptaRecord1.get_Field("ExpNumber").ToString());
                    workItem.TotalValueDisplay = workItem.TotalValue.ToString("N2");
                    workItem.CurrencyCode = string.Empty;
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine(axaptaRecord1.get_Field("Txt1").ToString());
                    stringBuilder.AppendLine("");
                    stringBuilder.AppendFormat("{0}", axaptaRecord1.get_Field("Txt2"));
                    stringBuilder.AppendLine("                                               ");
                    workItem.Description = stringBuilder.ToString();
                    AxaptaRecord axaptaRecord2 = ax.CreateAxaptaRecord("TrvExpTrans");
                    string str2 = string.Format("select * from %1 where %1.ExpNumber == '{0}'", (object)axaptaRecord1.get_Field("ExpNumber").ToString());
                    ax.ExecuteStmt(str2, axaptaRecord2);
                    while (axaptaRecord2.Found == true)
                    {
                        RequisitionItem reqItem = new RequisitionItem();
                        RequisitionItem requisitionItem1 = reqItem;
                        Decimal num2 = Convert.ToDecimal(axaptaRecord2.get_Field("AmountMST"));
                        string str3 = num2.ToString("N2");
                        requisitionItem1.UnitPrice = str3;
                        reqItem.Qty = new Decimal(1);
                        reqItem.Item = axaptaRecord2.get_Field("Description").ToString();
                        reqItem.CurrencyCode = string.Empty;
                        RequisitionItem requisitionItem2 = reqItem;
                        num2 = Convert.ToDecimal(axaptaRecord2.get_Field("AmountMST"));
                        string str4 = num2.ToString("N2");
                        requisitionItem2.LineAmount = str4;
                        num1 += Convert.ToDecimal(axaptaRecord2.get_Field("AmountMST"));
                        this.GetDimensions(ax, axaptaRecord2, reqItem, "ReferenceDataAreaId");
                        workItem._Items.Add(reqItem);
                        axaptaRecord2.Next();
                    }
                }
                catch (Exception ex)
                {
                    WorkItem workItem1 = workItem;
                    workItem1.Description = workItem1.Description + "err:" + ex.Message + " " + ex.StackTrace;
                }
            }
            workItem.TotalValue = num1;
            workItem.TotalValueDisplay = workItem.TotalValue.ToString("N2");
        }

        private void GetTrvExpTrans(
          Axapta ax,
          WorkItem workItem,
          string refTableId,
          string refRecId,
          string company)
        {
            AxaptaRecord axaptaRecord = ax.CreateAxaptaRecord("TrvExpTrans");
            string str1 = string.Format("select * from %1 where (%1.RecId == {0})", (object)refRecId, (object)company);
            ax.ExecuteStmt(str1, axaptaRecord);
            if (!axaptaRecord.Found)
                return;
            try
            {
                workItem.DateTime = Convert.ToDateTime(axaptaRecord.get_Field("TransDate")).ToString();
                workItem.TotalValue = Convert.ToDecimal(axaptaRecord.get_Field("AmountMST"));
                WorkItem workItem1 = workItem;
                Decimal totalValue = workItem.TotalValue;
                string str2 = totalValue.ToString("N2");
                workItem1.TotalValueDisplay = str2;
                workItem.CurrencyCode = string.Empty;
                RequisitionItem reqItem = new RequisitionItem();
                RequisitionItem requisitionItem1 = reqItem;
                totalValue = Convert.ToDecimal(axaptaRecord.get_Field("AmountMST"));
                string str3 = totalValue.ToString("N2");
                requisitionItem1.UnitPrice = str3;
                reqItem.Qty = new Decimal(1);
                reqItem.Item = axaptaRecord.get_Field("Description").ToString();
                reqItem.CurrencyCode = string.Empty;
                RequisitionItem requisitionItem2 = reqItem;
                totalValue = Convert.ToDecimal(axaptaRecord.get_Field("AmountMST"));
                string str4 = totalValue.ToString("N2");
                requisitionItem2.LineAmount = str4;
                this.GetDimensions(ax, axaptaRecord, reqItem, "ReferenceDataAreaId");
                workItem._Items.Add(reqItem);
            }
            catch (Exception ex)
            {
                WorkItem workItem1 = workItem;
                workItem1.Description = workItem1.Description + "err:" + ex.Message;
            }
        }

        private void GetTrvRequisitionTableTotalValue(
          Axapta ax,
          WorkItem workItem,
          string refTableId,
          string refRecId,
          string company)
        {
            AxaptaRecord axaptaRecord = ax.CreateAxaptaRecord("TrvRequisitionLine");
            string str = string.Format("select * from %1 where (%1.TrvRequisitionTable == {0})", (object)refRecId, (object)company);
            ax.ExecuteStmt(str, axaptaRecord);
            if (!axaptaRecord.Found == true)
                return;
            Decimal num = new Decimal(0);
            while (axaptaRecord.Found == true)
            {
                num += Convert.ToDecimal(axaptaRecord.get_Field("TransactionCurrencyAmount"));
                axaptaRecord.Next();
            }
            try
            {
                workItem.TotalValue = num;
                workItem.TotalValueDisplay = workItem.TotalValue.ToString("N2");
                workItem.CurrencyCode = string.Empty;
            }
            catch (Exception ex)
            {
                WorkItem workItem1 = workItem;
                workItem1.Description = workItem1.Description + "err:" + ex.Message;
            }
        }

        private void GetTrvRequisition(
          Axapta ax,
          WorkItem workItem,
          string refTableId,
          string refRecId,
          string company)
        {
            AxaptaRecord axaptaRecord1 = ax.CreateAxaptaRecord("TrvRequisitionTable");
            string str1 = string.Format("select * from %1 where (%1.RecId == {0})", (object)refRecId, (object)company);
            ax.ExecuteStmt(str1, axaptaRecord1);
            if (!axaptaRecord1.Found == true)
                return;
            try
            {
                workItem.DateTime = Convert.ToDateTime(axaptaRecord1.get_Field("RequisitionDate")).ToString();
                workItem.CurrencyCode = string.Empty;
                AxaptaRecord axaptaRecord2 = ax.CreateAxaptaRecord("TrvRequisitionLine");
                string str2 = string.Format("select * from %1 where (%1.TrvRequisitionTable == {0})", (object)refRecId, (object)company);
                ax.ExecuteStmt(str2, axaptaRecord2);
                while (axaptaRecord2.Found == true)
                {
                    RequisitionItem reqItem = new RequisitionItem();
                    RequisitionItem requisitionItem1 = reqItem;
                    Decimal num = Convert.ToDecimal(axaptaRecord2.get_Field("TransactionCurrencyAmount"));
                    string str3 = num.ToString("N2");
                    requisitionItem1.UnitPrice = str3;
                    reqItem.Qty = new Decimal(1);
                    reqItem.Item = axaptaRecord2.get_Field("Category").ToString();
                    reqItem.CurrencyCode = string.Empty;
                    RequisitionItem requisitionItem2 = reqItem;
                    num = Convert.ToDecimal(axaptaRecord2.get_Field("TransactionCurrencyAmount"));
                    string str4 = num.ToString("N2");
                    requisitionItem2.LineAmount = str4;
                    this.GetDimensions(ax, axaptaRecord2, reqItem, "ReferenceDataAreaId");
                    workItem._Items.Add(reqItem);
                    axaptaRecord2.Next();
                }
            }
            catch (Exception ex)
            {
                WorkItem workItem1 = workItem;
                workItem1.Description = workItem1.Description + "err:" + ex.Message;
            }
        }

        private void GetTrvCashAdvance(
          Axapta ax,
          WorkItem workItem,
          string refTableId,
          string refRecId,
          string company)
        {
            AxaptaRecord axaptaRecord = ax.CreateAxaptaRecord("TrvCashAdvance");
            string str1 = string.Format("select * from %1 where (%1.RecId == {0})", (object)refRecId, (object)company);
            ax.ExecuteStmt(str1, axaptaRecord);
            Decimal num1 = new Decimal(0);
            if (axaptaRecord.Found == true)
            {
                try
                {
                    RequisitionItem reqItem = new RequisitionItem();
                    RequisitionItem requisitionItem1 = reqItem;
                    Decimal num2 = Convert.ToDecimal(axaptaRecord.get_Field("RequestedAmountCur"));
                    string str2 = num2.ToString("N2");
                    requisitionItem1.UnitPrice = str2;
                    reqItem.Qty = new Decimal(1);
                    reqItem.Item = axaptaRecord.get_Field("Purpose").ToString();
                    reqItem.CurrencyCode = axaptaRecord.get_Field("CurrencyCode").ToString();
                    reqItem.CurrencyCode = string.Empty;
                    RequisitionItem requisitionItem2 = reqItem;
                    num2 = Convert.ToDecimal(axaptaRecord.get_Field("RequestedAmountCur"));
                    string str3 = num2.ToString("N2");
                    requisitionItem2.LineAmount = str3;
                    num1 += Convert.ToDecimal(axaptaRecord.get_Field("RequestedAmountCur"));
                    this.GetDimensions(ax, axaptaRecord, reqItem, "DataAreaId");
                    workItem._Items.Add(reqItem);
                }
                catch (Exception ex)
                {
                    WorkItem workItem1 = workItem;
                    workItem1.Description = workItem1.Description + "err:" + ex.Message;
                }
            }
            workItem.TotalValue = num1;
            workItem.TotalValueDisplay = workItem.TotalValue.ToString("N2");
        }

        private void GetLedgerTableInfo(
          Axapta ax,
          WorkItem workItem,
          string refTableId,
          string refRecId,
          string company)
        {
            AxaptaRecord axaptaRecord1 = ax.CreateAxaptaRecord("LedgerJournalTable");
            string str1 = string.Format("select %1 where (%1.RecId == {0})", (object)refRecId, (object)company);
            ax.ExecuteStmt(str1, axaptaRecord1);
            if (!axaptaRecord1.Found == true)
                return;
            string field = (string)axaptaRecord1.get_Field("JournalNum");
            AxaptaRecord axaptaRecord2 = ax.CreateAxaptaRecord("LedgerJournalTrans");
            string str2 = string.Format("select %1 where %1.JournalNum == \"{0}\"", (object)field, (object)company);
            ax.ExecuteStmt(str2, axaptaRecord2);
            Decimal num1 = new Decimal(0);
            Decimal num2 = new Decimal(0);
            while (axaptaRecord2.Found == true)
            {
                RequisitionItem reqItem = new RequisitionItem();
                reqItem.UnitPrice = (Convert.ToDecimal(axaptaRecord2.get_Field("AmountCurDebit")) > Convert.ToDecimal(axaptaRecord2.get_Field("AmountCurCredit")) ? Convert.ToDecimal(axaptaRecord2.get_Field("AmountCurDebit")) : Convert.ToDecimal(axaptaRecord2.get_Field("AmountCurCredit"))).ToString("N2");
                reqItem.Qty = new Decimal(1);
                reqItem.Item = "Line " + Convert.ToDouble(axaptaRecord2.get_Field("LineNum")).ToString("N0");
                reqItem.CurrencyCode = this.GetSymbolForCurrency(ax, axaptaRecord2.get_Field("CurrencyCode").ToString(), company);
                reqItem.LineAmount = (Convert.ToDecimal(axaptaRecord2.get_Field("AmountCurDebit")) > Convert.ToDecimal(axaptaRecord2.get_Field("AmountCurCredit")) ? Convert.ToDecimal(axaptaRecord2.get_Field("AmountCurDebit")) : Convert.ToDecimal(axaptaRecord2.get_Field("AmountCurCredit"))).ToString("N2");
                reqItem.ItemDate = Convert.ToDateTime(axaptaRecord2.get_Field("TransDate")).ToString();
                this.GetDimensions(ax, axaptaRecord2, reqItem, "DataAreaId");
                workItem._Items.Add(reqItem);
                num1 += Convert.ToDecimal(axaptaRecord2.get_Field("AmountCurDebit"));
                num2 += Convert.ToDecimal(axaptaRecord2.get_Field("AmountCurCredit"));
                axaptaRecord2.Next();
            }
            workItem.TotalValue = Math.Abs(num2 - num1);
            workItem.TotalValueDisplay = workItem.TotalValue.ToString("N2");
        }

        private void GetLedgerTableInfoTotalValue(
          Axapta ax,
          WorkItem workItem,
          string refTableId,
          string refRecId,
          string company)
        {
            AxaptaRecord axaptaRecord1 = ax.CreateAxaptaRecord("LedgerJournalTable");
            string str1 = string.Format("select  %1 where (%1.RecId == {0})", (object)refRecId, (object)company);
            ax.ExecuteStmt(str1, axaptaRecord1);
            if (!axaptaRecord1.Found == true)
                return;
            string field = (string)axaptaRecord1.get_Field("JournalNum");
            AxaptaRecord axaptaRecord2 = ax.CreateAxaptaRecord("LedgerJournalTrans");
            string str2 = string.Format("select %1 where %1.JournalNum == \"{0}\"", (object)field, (object)company);
            ax.ExecuteStmt(str2, axaptaRecord2);
            Decimal num1 = new Decimal(0);
            Decimal num2 = new Decimal(0);
            while (axaptaRecord2.Found == true)
            {
                num1 += Convert.ToDecimal(axaptaRecord2.get_Field("AmountCurDebit"));
                num2 += Convert.ToDecimal(axaptaRecord2.get_Field("AmountCurCredit"));
                axaptaRecord2.Next();
            }
            workItem.TotalValue = Math.Abs(num2 - num1);
            workItem.TotalValueDisplay = workItem.TotalValue.ToString("N2");
        }

        private void GetTSTimeSheet(
          Axapta ax,
          WorkItem workItem,
          string refTableId,
          string refRecId,
          string company)
        {
            AxaptaRecord axaptaRecord1 = ax.CreateAxaptaRecord("TSTimesheetTable");
            string str1 = string.Format("select %1 where (%1.RecId == {0})", (object)refRecId, (object)company);
            ax.ExecuteStmt(str1, axaptaRecord1);
            string empty = string.Empty;
            StringBuilder stringBuilder1 = new StringBuilder();
            if (!axaptaRecord1.Found == true)
                return;
            string str2 = axaptaRecord1.get_Field("TimeSheetNbr").ToString();
            AxaptaRecord axaptaRecord2 = ax.CreateAxaptaRecord("TSTimeSheetSummaryLine");
            AxaptaRecord axaptaRecord3 = ax.CreateAxaptaRecord("ProjTable");
            AxaptaRecord axaptaRecord4 = ax.CreateAxaptaRecord("SmmActivities");
            string str3 = string.Format("select %1 join %2 join %3 where %1.ProjId == %2.ProjId && %1.TimeSheetNbr == '{0}' && %1.ActivityNumber == %3.ActivityNumber", (object)str2, (object)axaptaRecord3, (object)axaptaRecord4);
            ax.ExecuteStmt(str3, axaptaRecord2, axaptaRecord3, axaptaRecord4);
            StringBuilder stringBuilder2 = new StringBuilder();
            int num1 = 0;
            Decimal[] numArray = new Decimal[8];
            DateTime dateTime = new DateTime();
            DateTime currentDate;
            while (axaptaRecord2.Found == true)
            {
                if (num1 > 0)
                    stringBuilder2.AppendLine("");
                AxaptaRecord axaptaRecord5 = ax.CreateAxaptaRecord("DirPersonUser");
                AxaptaRecord axaptaRecord6 = ax.CreateAxaptaRecord("HcmWorker");
                string str4 = string.Format("Select * from %1 join %2 where %1.PersonParty == %2.Person && %2.RecId == {0}", axaptaRecord2.get_Field("Worker"));
                ax.ExecuteStmt(str4, axaptaRecord5, axaptaRecord6);
                if (axaptaRecord5.Found == true)
                {
                    empty = axaptaRecord5.get_Field("User").ToString();
                    stringBuilder2.AppendFormat("{0}: {1}", (object)this.GetLabelDescription(ax, "@SYS54564"), (object)empty);
                    stringBuilder2.AppendLine("");
                }
                stringBuilder2.AppendFormat("{0}: {1}", (object)this.GetLabelDescription(ax, "@SYS4534"), (object)axaptaRecord3.get_Field("Name").ToString());
                stringBuilder2.AppendLine("");
                stringBuilder2.AppendFormat("{0}: {1} ({2})", (object)this.GetLabelDescription(ax, "@SYS1695"), (object)axaptaRecord4.get_Field("Purpose").ToString(), (object)axaptaRecord2.get_Field("ActivityNumber").ToString());
                stringBuilder2.AppendLine("");
                stringBuilder2.AppendFormat("{0}: {1}", (object)this.GetLabelDescription(ax, "@SYS11718"), (object)axaptaRecord2.get_Field("CategoryId").ToString());
                stringBuilder2.AppendLine("");
                stringBuilder2.AppendLine("");
                dateTime = Convert.ToDateTime(axaptaRecord2.get_Field("DayFrom"));
                currentDate = dateTime;
                Decimal num2;
                for (int index = 1; index <= 7; ++index)
                {
                    string str5 = string.Empty;
                    if (index > 1)
                        str5 = "[" + (object)index + "]";
                    StringBuilder stringBuilder3 = stringBuilder2;
                    num2 = Convert.ToDecimal(axaptaRecord2.get_Field("HOURS" + str5));
                    string str6 = num2.ToString("N2");
                    string str7 = currentDate.DayOfWeek.ToString().Substring(0, 3);
                    string str8 = currentDate.ToString("yyyy/MM/dd");
                    stringBuilder3.AppendFormat("{0} Hs | ({1}) {2} ", (object)str6, (object)str7, (object)str8);
                    stringBuilder2.AppendLine("");
                    stringBuilder2.Append(this.GetTimesheetCommentsForDate(ax, axaptaRecord2.get_Field("TimesheetNbr").ToString(), currentDate));
                    currentDate = currentDate.AddDays(1.0);
                    numArray[index] += Convert.ToDecimal(axaptaRecord2.get_Field("HOURS" + str5));
                }
                StringBuilder stringBuilder4 = stringBuilder2;
                string labelDescription = this.GetLabelDescription(ax, "@PSA1445");
                num2 = Convert.ToDecimal(axaptaRecord2.get_Field("TotalHourSum"));
                string str9 = num2.ToString("N2");
                stringBuilder4.AppendFormat("{0}: {1} Hs", (object)labelDescription, (object)str9);
                stringBuilder2.AppendLine("");
                stringBuilder1.AppendFormat("{0}, ", (object)axaptaRecord3.get_Field("Name").ToString());
                axaptaRecord2.Next();
                ++num1;
            }
            if (num1 > 1)
            {
                stringBuilder2.AppendLine();
                stringBuilder2.AppendLine();
                stringBuilder2.AppendFormat("Timesheet Summary:");
                stringBuilder2.AppendLine();
                stringBuilder2.AppendLine();
                currentDate = dateTime;
                Decimal num2 = new Decimal(0);
                for (int index = 1; index <= 7; ++index)
                {
                    stringBuilder2.AppendFormat("Total of {0} Hs | ({1}) {2} ", (object)numArray[index].ToString("N2"), (object)currentDate.DayOfWeek.ToString().Substring(0, 3), (object)currentDate.ToString("yyyy/MM/dd"));
                    stringBuilder2.AppendLine();
                    currentDate = currentDate.AddDays(1.0);
                    num2 += numArray[index];
                }
                stringBuilder2.AppendLine();
                stringBuilder2.AppendFormat("Timesheet Total: {0} Hs", (object)num2.ToString("N2"));
            }
            if (stringBuilder1.Length > 0)
                stringBuilder1 = stringBuilder1.Remove(stringBuilder1.Length - 2, 2);
            workItem.Description = stringBuilder2.ToString();
            workItem.Name = string.Format("{0}: {1} | {2}: {3}", (object)this.GetLabelDescription(ax, "@SYS54564"), (object)empty, (object)this.GetLabelDescription(ax, "@SYS36368"), (object)stringBuilder1.ToString());
        }

        private string GetTimesheetCommentsForDate(
          Axapta ax,
          string timesheetNumber,
          DateTime currentDate)
        {
            StringBuilder stringBuilder = new StringBuilder();
            AxaptaRecord axaptaRecord = ax.CreateAxaptaRecord("TsTimeSheetTrans");
            string str = string.Format("select %1 where %1.TimeSheetNbr == '{0}' && %1.ProjTransDate == str2Date('{1}', 123)", (object)timesheetNumber, (object)currentDate.ToString("dd/MM/yyyy"));
            ax.ExecuteStmt(str, axaptaRecord);
            if (axaptaRecord.Found == true && !string.IsNullOrEmpty(axaptaRecord.get_Field("IntComment").ToString()))
            {
                stringBuilder.AppendFormat("{0}: {1}", (object)this.GetLabelDescription(ax, "@PSA343"), axaptaRecord.get_Field("IntComment"));
                stringBuilder.AppendLine();
            }
            return stringBuilder.ToString();
        }

        private void GetTSTimeSheetTotalValue(
          Axapta ax,
          WorkItem workItem,
          string refTableId,
          string refRecId,
          string company)
        {
            AxaptaRecord axaptaRecord1 = ax.CreateAxaptaRecord("TSTimesheetTable");
            string str1 = string.Format("select %1 where (%1.RecId == {0})", (object)refRecId, (object)company);
            ax.ExecuteStmt(str1, axaptaRecord1);
            if (!axaptaRecord1.Found == true)
                return;
            string str2 = axaptaRecord1.get_Field("TimeSheetNbr").ToString();
            AxaptaRecord axaptaRecord2 = ax.CreateAxaptaRecord("TSTimeSheetSummaryLine");
            AxaptaRecord axaptaRecord3 = ax.CreateAxaptaRecord("ProjTable");
            string str3 = string.Format("select %1 join %2 where %1.ProjId == %2.ProjId && %1.TimeSheetNbr == '{0}'", (object)str2, (object)axaptaRecord3);
            ax.ExecuteStmt(str3, axaptaRecord2, axaptaRecord3);
            if (axaptaRecord2.Found == true)
            {
                workItem.TotalValue = Convert.ToDecimal(axaptaRecord2.get_Field("TotalHourSum"));
                workItem.TotalValueDisplay = Convert.ToDecimal(axaptaRecord2.get_Field("TotalHourSum")).ToString("N2");
                AxaptaRecord axaptaRecord4 = ax.CreateAxaptaRecord("DirPersonUser");
                AxaptaRecord axaptaRecord5 = ax.CreateAxaptaRecord("HcmWorker");
                string str4 = string.Format("Select * from %1 join %2 where %1.PersonParty == %2.Person && %2.RecId == {0}", axaptaRecord2.get_Field("Worker"));
                ax.ExecuteStmt(str4, axaptaRecord4, axaptaRecord5);
                StringBuilder stringBuilder = new StringBuilder();
                while (axaptaRecord2.Found == true)
                {
                    stringBuilder.AppendFormat("{0}, ", axaptaRecord3.get_Field("Name"));
                    axaptaRecord2.Next();
                }
                if (stringBuilder.Length > 0)
                    stringBuilder.Remove(stringBuilder.Length - 2, 2);
                if (axaptaRecord4.Found == true)
                    workItem.Name = string.Format("{0}: {1} | {2}: {3}", (object)this.GetLabelDescription(ax, "@SYS54564"), (object)axaptaRecord4.get_Field("User").ToString(), (object)this.GetLabelDescription(ax, "@SYS36368"), (object)stringBuilder.ToString());
            }
        }

        private DateTime GetDateApplyTimezone(Axapta ax, DateTime dt)
        {
            object obj = ax.CallStaticClassMethod("DateTimeUtil", "getUserPreferredTimeZone");
            return Convert.ToDateTime(ax.CallStaticClassMethod("DateTimeUtil", "applyTimeZoneOffset", (object)dt, obj));
        }

        private string[] GetTablesRemove()
        {
            string appSetting = ConfigurationManager.AppSettings["tablesRemove"];
            if (string.IsNullOrEmpty(appSetting))
                return (string[])null;
            return appSetting.Split(';');
        }

        private bool inTablesRemove(string[] tablesRemove, object tblName)
        {
            if (tablesRemove != null && tablesRemove.Length > 0)
            {
                foreach (string str in tablesRemove)
                {
                    if (str.Trim().ToLower() == tblName.ToString().ToLower())
                        return true;
                }
            }
            return false;
        }


        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        [WebMethod]
        public List<WorkType> GetWorkTypes(
       string userName,
       string password,
       string domain,
       string companies,
       string utcTime,
       string signatureValue,
       bool includeItems)
        {
            this.ValidateSignature(userName, password, utcTime, signatureValue);
            password = new SecurityHelper().Decrypt(password, true);

            if (this.GetAxInstance(userName, password, domain, "") == null)
            {
                throw new Exception("It's impossible to instanciate AX");
            }

            using (Axapta ax = this.GetAxInstance(userName, password, domain, ""))
            {
                List<WorkType> list = new List<WorkType>();
                string[] tablesRemove = this.GetTablesRemove();
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
                                if (this.IsDataAreaIncluded(empty, companies))
                                {
                                    object contextTableId = workflowTrackingStatusTableRecord.get_Field("ContextTableId");
                                    object documentType = workflowTrackingStatusTableRecord.get_Field("DocumentType");
                                    object recId = workflowTrackingStatusTableRecord.get_Field("RecId");
                                    object tblName = ax.CallStaticClassMethod("Global", "tableId2Name", recId);

                                    list.Add(new WorkType() { WorkTypeName = contextTableId.ToString(), WorkTypeCount = Convert.ToInt32(recId) });
                                    workflowWorkItemTableRecord.Next();
                                }
                            }
                        }
                    }
                }
                return list;
            }
        }


    }

}
