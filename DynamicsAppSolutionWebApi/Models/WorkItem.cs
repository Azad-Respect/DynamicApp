using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicsAppSolutionWebApi.Models
{
    public class WorkItem
    {
        public List<RequisitionItem> _Items { get; set; }

        public List<HistoricalItem> HistoricalItems { get; set; }

        public List<Action> Actions { get; set; }

        public string ActionsString { get; set; }

        public string DateTime { get; set; }

        public string Justification { get; set; }

        public string Name { get; set; }

        public double RecId { get; set; }

        public string Requisitioner { get; set; }

        public string Status { get; set; }

        public string Subject { get; set; }

        public string Description { get; set; }

        public string TableName { get; set; }

        public string TableLabel { get; set; }

        public string TemplateName { get; set; }

        public string TemplateApprove { get; set; }

        public string TemplateReject { get; set; }

        public Decimal TotalValue { get; set; }

        public string TotalValueDisplay { get; set; }

        public string Type { get; set; }

        public string ImageURL { get; set; }

        public string ElementName { get; set; }

        public int ElementType { get; set; }

        public string DataAreaId { get; set; }

        public string DataAreaName { get; set; }

        public string CurrencyCode { get; set; }

        public int AttachmentsCount { get; set; }

        public string Originator { get; set; }
    }
}