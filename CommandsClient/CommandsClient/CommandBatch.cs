using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace CommandsClient
{
    public static class Commands
    {
        public const string AddAccountingEntryLineCommand = "AddAccountingEntryLine";
        public const string AddPicklistKeywordCommand = "AddPicklistKeyword";
        public const string ClearDefaultResourceCommand = "ClearDefaultResource";
        public const string ClearPicklistCommand = "ClearPicklist";
        public const string CreateCaseCommand = "CreateCase";
        public const string CreateContractCommand = "CreateContract";
        public const string CreateHierarchyCommand = "CreateHierarchy";
        public const string CreateIndividualCommand = "CreateIndividual";
        public const string CreatePlanCommand = "CreatePlan";
        public const string CreateProductCommand = "CreateProduct";
        public const string CreateResourceCommand = "CreateResource";
        public const string CreateScheduleCommand = "CreateSchedule";
        public const string CreateTraitCommand = "CreateTrait";
        public const string CreateUnitCommand = "CreateUnit";
        public const string CreateUsageQuantityCommand = "CreateUsageQuantity";
        public const string CreateWorkOrderCommand = "CreateWorkOrder";
        public const string DeletePicklistKeywordCommand = "DeletePicklistKeyword";
        public const string InvoiceInvoiceCommand = "InvoiceInvoice";
        public const string InvoiceReadyForInvoiceCommand = "InvoiceReadyForInvoice";
        public const string SetDefaultResourceCommand = "SetDefaultResource";
        public const string UpdateCaseCommand = "UpdateCase";
        public const string UpdateContractCommand = "UpdateContract";
        public const string UpdateHierarchyCommand = "UpdateHierarchy";
        public const string UpdateIndividualCommand = "UpdateIndividual";
        public const string UpdateInvoiceRowCommand = "UpdateInvoiceRow";
        public const string UpdateLocalizationKeywordCommand = "UpdateLocalizationKeyword";
        public const string UpdatePicklistKeywordCommand = "UpdatePicklistKeyword";
        public const string UpdatePlanCommand = "UpdatePlan";
        public const string UpdatePriceAdjustmentCommand = "UpdatePriceAdjustment";
        public const string UpdateProductCommand = "UpdateProduct";
        public const string UpdateResourceCommand = "UpdateResource";
        public const string UpdateScheduleCommand = "UpdateSchedule";
        public const string UpdateTraitCommand = "UpdateTrait";
        public const string UpdateUnitCommand = "UpdateUnit";
        public const string UpdateUsageQuantityCommand = "UpdateUsageQuantity";
        public const string UpdateWorkOrderCommand = "UpdateWorkOrder";
        public const string UpsertCaseCommand = "UpsertCase";
        public const string UpsertContractCommand = "UpsertContract";
        public const string UpsertHierarchyCommand = "UpsertHierarchy";
        public const string UpsertIndividualCommand = "UpsertIndividual";
        public const string UpsertPriceAdjustmentCommand = "UpsertPriceAdjustment";
        public const string UpsertProductCommand = "UpsertProduct";
        public const string UpsertResourceCommand = "UpsertResource";
        public const string UpsertTraitCommand = "UpsertTrait";
        public const string UpsertUnitCommand = "UpsertUnit";
        public const string UpsertUsageQuantityCommand = "UpsertUsageQuantity";
        public const string UpsertUserCommand = "UpsertUser";
        public const string UpsertWorkOrderCommand = "UpsertWorkOrder";
        public const string WriteToLogCommand = "WriteToLog";

        public static Command InvoiceInvoice(string target)
        {
            return new Command
            {
                Name = InvoiceInvoiceCommand,
                Target = target
            };
        }

        public static Command SetDefaultResource(string target)
        {
            return new Command
            {
                Name = SetDefaultResourceCommand,
                Target = target
            };
        }

        public static Command ClearDefaultResource(string target)
        {
            return new Command
            {
                Name = ClearDefaultResourceCommand,
                Target = target
            };
        }

        public static Command UpdateWorkOrder(string target)
        {
            return new Command
            {
                Name = UpdateWorkOrderCommand,
                Target = target
            };
        }

        public static Command UpsertWorkOrder(string target)
        {
            return new Command
            {
                Name = UpsertWorkOrderCommand,
                Target = target
            };
        }

        public static Command CreateCaseWithWorkorder(string title, string unit, string resourceId)
        {
            var parameters = new List<CommandParameter>
			{
				new CommandParameter( "Title", title ),
				new CommandParameter( "Unit", unit )
			};

            if (null != resourceId)
                parameters.Add(new CommandParameter("WorkorderResource", resourceId));

            return new Command
            {
                Name = CreateCaseCommand,
                Parameters = parameters.ToArray()
            };
        }

        public static Command CreateCase(string title, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter>
			{
				new CommandParameter( "Title", title )
			};

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command
            {
                Name = CreateCaseCommand,
                Parameters = parameters.ToArray()
            };
        }

        public static Command CreateCase(string title)
        {
            return CreateCase(title, new Dictionary<string, string>());
        }

        public static Command UpsertCase(string target, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter>();

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command
            {
                Name = UpsertCaseCommand,
                Target = target,
                Parameters = parameters.ToArray()
            };
        }

        public static Command UpsertCase(string target)
        {
            return new Command
            {
                Name = UpsertCaseCommand,
                Target = target,
            };
        }

        public static Command UpdateCase(string target, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter>();

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command
            {
                Name = UpdateCaseCommand,
                Target = target,
                Parameters = parameters.ToArray()
            };
        }


        public static Command UpdateCase(string target)
        {
            return new Command
            {
                Name = UpdateCaseCommand,
                Target = target,
            };
        }

        public static Command WriteToLog(string targetHref, string logMessage)
        {
            return WriteToLog(targetHref, logMessage, null);
        }

        public static Command WriteToLog(string targetHref, string logMessage, string category)
        {
            var parameters = new List<CommandParameter>
			{
				new CommandParameter( "Text", logMessage )
			};

            if (null != category)
                parameters.Add(new CommandParameter("Category", category));
            return new Command
            {
                Name = WriteToLogCommand,
                Target = targetHref,
                Parameters = parameters.ToArray()
            };
        }

        //public static Command UpdateUsageQuantity(string target, UsageQuantityStatus newStatus)
        //{
        //    return UpdateUsageQuantity(target, new Dictionary<string, string> { { "Status", newStatus.ToString() } });
        //}

        public static Command UpdateUsageQuantity(string target, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter>();

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command
            {
                Name = UpdateUsageQuantityCommand,
                Target = target,
                Parameters = parameters.ToArray()
            };
        }

        public static Command UpsertUsageQuantity(string target, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter>();

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command
            {
                Name = UpsertUsageQuantityCommand,
                Target = target,
                Parameters = parameters.ToArray()
            };
        }

        public static Command CreateHierarchy(string title, string description)
        {
            var parameters = new List<CommandParameter>
			{
				new CommandParameter( "Title", title )
			};

            if (null != description)
                parameters.Add(new CommandParameter("Description", description));

            return new Command
            {
                Name = CreateHierarchyCommand,
                Parameters = parameters.ToArray()
            };
        }

        public static Command CreateHierarchy(string title)
        {
            return CreateHierarchy(title, null);
        }

        public static Command UpdateHierarchy(string target)
        {
            return new Command { Name = UpdateHierarchyCommand, Target = target };
        }

        public static Command UpsertHierarchy(string target)
        {
            return new Command { Name = UpsertHierarchyCommand, Target = target };
        }

        //public static Command CreateUnit(string title, string description, string contractRestId, Tag[] tags)
        //{
        //    var parameters = new List<CommandParameter>
        //    {
        //        new CommandParameter( "Title", title )
        //    };

        //    if (null != description)
        //        parameters.Add(new CommandParameter("Description", description));
        //    if (null != contractRestId)
        //        parameters.Add(new CommandParameter("Contract", contractRestId));
        //    if (null != tags)
        //        parameters.AddRange(tags.Select(tag => new CommandParameter("Tag." + tag.Name, tag.Response)));

        //    return new Command
        //    {
        //        Name = CreateUnitCommand,
        //        Parameters = parameters.ToArray()
        //    };
        //}

        public static Command CreateUnit(string title, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter>
			{
				new CommandParameter( "Title", title )
			};

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command
            {
                Name = CreateUnitCommand,
                Parameters = parameters.ToArray()
            };
        }

        public static Command UpdateContract(string target, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter>();

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command
            {
                Name = UpdateContractCommand,
                Target = target,
                Parameters = parameters.ToArray()
            };
        }

        public static Command UpdateContract(string target)
        {
            return UpdateContract(target, new Dictionary<string, string>());
        }

        //public static Command CreateUnit(string title, string description, string contractRestId)
        //{
        //    return CreateUnit(title, description, contractRestId, null);
        //}

        //public static Command CreateUnit(string title, string description)
        //{
        //    return CreateUnit(title, description, null);
        //}

        //public static Command CreateUnit(string title)
        //{
        //    return CreateUnit(title, String.Empty);
        //}

        public static Command UpdateUnit(string target)
        {
            return UpdateUnit(target, new Dictionary<string, string>());
        }

        public static Command UpdateUnit(string target, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter>();

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command
            {
                Name = UpdateUnitCommand,
                Target = target,
                Parameters = parameters.ToArray()
            };
        }

        public static Command SetUnitHierarchy(string target, string hierarchy)
        {
            var parameters = new List<CommandParameter>
			{
				new CommandParameter( "Hierarchy", hierarchy )
			};

            return new Command
            {
                Name = UpdateUnitCommand,
                Target = target,
                Parameters = parameters.ToArray()
            };
        }

        public static Command CreateProduct(string title, string type, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter>
			{
				new CommandParameter( "Title", title ),
				new CommandParameter( "Type", type )
			};

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command
            {
                Name = CreateProductCommand,
                Parameters = parameters.ToArray()
            };
        }

        public static Command CreateProduct(string title, string type)
        {
            return CreateProduct(title, type, new Dictionary<string, string>());
        }

        public static Command UpdateProduct(string target, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter>();

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command
            {
                Name = UpdateProductCommand,
                Target = target,
                Parameters = parameters.ToArray()
            };
        }

        public static Command UpdateProduct(string target)
        {
            return UpdateProduct(target, new Dictionary<string, string>());
        }

        public static Command UpdatePriceAdjustment(string target)
        {
            return UpdatePriceAdjustment(target, new Dictionary<string, string>());
        }

        public static Command UpdatePriceAdjustment(string target, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter>();

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command
            {
                Name = UpdatePriceAdjustmentCommand,
                Target = target,
                Parameters = parameters.ToArray()
            };
        }

        public static Command CreateContract(string title)
        {
            return CreateContract(title, new Dictionary<string, string>());
        }

        public static Command CreateContract(string title, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter>
			{
				new CommandParameter( "Title", title )
			};

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command
            {
                Name = CreateContractCommand,
                Parameters = parameters.ToArray()
            };
        }

        public static Command CreateIndividual()
        {
            return CreateIndividual(new Dictionary<string, string>());
        }

        public static Command CreateIndividual(Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter>();

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command
            {
                Name = CreateIndividualCommand,
                Parameters = parameters.ToArray()
            };
        }

        public static Command UpdateIndividual(string target, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter>();

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command
            {
                Name = UpdateIndividualCommand,
                Target = target,
                Parameters = parameters.ToArray()
            };
        }

        public static Command UpdateIndividual(string target)
        {
            return UpdateIndividual(target, new Dictionary<string, string>());
        }

        public static Command CreateTrait(string title, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter>
			{
				new CommandParameter( "Title", title )
			};

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command
            {
                Name = CreateTraitCommand,
                Parameters = parameters.ToArray()
            };
        }

        public static Command CreateTrait(string title)
        {
            return CreateTrait(title, new Dictionary<string, string>());
        }

        public static Command UpdateTrait(string target, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter>();

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command
            {
                Name = UpdateTraitCommand,
                Target = target,
                Parameters = parameters.ToArray()
            };
        }

        public static Command UpdateTrait(string target)
        {
            return UpdateTrait(target, new Dictionary<string, string>());
        }

        public static Command CreateWorkOrder(string parentCase, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter> { new CommandParameter("Case", parentCase) };

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command
            {
                Name = CreateWorkOrderCommand,
                Parameters = parameters.ToArray()
            };
        }

        public static Command CreateWorkOrder(string title, string parentCase)
        {
            return CreateWorkOrder(parentCase, new Dictionary<string, string> { { "Title", title } });
        }

        public static Command CreateWorkOrder(string parentCase)
        {
            return CreateWorkOrder(parentCase, new Dictionary<string, string>());
        }

        public static Command UpsertUnit(string target)
        {
            return UpsertUnit(target, new Dictionary<string, string>());
        }

        public static Command UpsertTrait(string target)
        {
            return UpsertTrait(target, new Dictionary<string, string>());
        }

        public static Command UpsertIndividual(string target)
        {
            return UpsertIndividual(target, new Dictionary<string, string>());
        }

        public static Command UpsertUser(string username)
        {
            if (String.IsNullOrEmpty(username))
                throw new ArgumentNullException("username");
            return new Command { Name = UpsertUserCommand, Target = "users/" + username }.Parameter("Username", username);
        }

        public static Command UpsertUser(string username, string password)
        {
            if (String.IsNullOrEmpty(username))
                throw new ArgumentNullException("username");
            if (String.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");
            return
                new Command { Name = UpsertUserCommand, Target = "users/" + username }.Parameter("Username", username).Parameter(
                    "PasswordHash",
                    SHA1.Create().ComputeHash(Encoding.Default.GetBytes(password)).Aggregate(
                        "", (s, b) => string.Format(CultureInfo.InvariantCulture, "{0}{1:X2}", s, b)));
        }

        public static Command ClearPicklist(string target)
        {
            return new Command { Name = ClearPicklistCommand, Target = target };
        }

        public static Command AddPicklistKeyword(string target, string key, string text)
        {
            var parameters = new List<CommandParameter>
			{
				new CommandParameter( "Key", key ),
				new CommandParameter( "Text", text )
			};

            return new Command
            {
                Name = AddPicklistKeywordCommand,
                Parameters = parameters.ToArray(),
                Target = target
            };
        }

        public static Command UpdatePicklistKeyword(string targetPicklistAndKey, Dictionary<string, string> parameterMap)
        {
            return new Command
            {
                Name = UpdatePicklistKeywordCommand,
                Parameters = parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)).ToArray(),
                Target = targetPicklistAndKey
            };
        }

        public static Command DeletePicklistKeyword(string targetPicklistAndKey)
        {
            return new Command
            {
                Name = DeletePicklistKeywordCommand,
                Target = targetPicklistAndKey
            };
        }

        public static Command CreateUsageQuantity(
            string targetCase, string targetResource, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter>
			{
				new CommandParameter( "Case", targetCase ),
				new CommandParameter( "Resource", targetResource )
			};

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command
            {
                Name = CreateUsageQuantityCommand,
                Parameters = parameters.ToArray()
            };
        }

        public static Command CreateResource(string displayName, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter>
			{
				new CommandParameter( "DisplayName", displayName )
			};

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command
            {
                Name = CreateResourceCommand,
                Parameters = parameters.ToArray()
            };
        }

        public static Command CreateResource(string displayName)
        {
            return CreateResource(displayName, new Dictionary<string, string>());
        }

        public static Command UpdateResource(string target, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter>();

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command
            {
                Name = UpdateResourceCommand,
                Target = target,
                Parameters = parameters.ToArray()
            };
        }

        public static Command UpdateResource(string target)
        {
            return UpdateResource(target, new Dictionary<string, string>());
        }

        public static Command UpsertResource(string target, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter>();

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command
            {
                Name = UpsertResourceCommand,
                Target = target,
                Parameters = parameters.ToArray()
            };
        }

        public static Command UpsertResource(string target)
        {
            return UpsertResource(target, new Dictionary<string, string>());
        }

        public static Command CreatePlan(string title, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter>
			{
				new CommandParameter( "Title", title )
			};

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command
            {
                Name = CreatePlanCommand,
                Parameters = parameters.ToArray()
            };
        }

        public static Command CreatePlan(string title)
        {
            return CreatePlan(title, new Dictionary<string, string>());
        }

        public static Command CreateSchedule(string parentPlan)
        {
            return CreateSchedule(parentPlan, new Dictionary<string, string>());
        }

        public static Command CreateSchedule(string parentPlan, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter> { new CommandParameter("Plan", parentPlan) };

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command
            {
                Name = CreateScheduleCommand,
                Parameters = parameters.ToArray()
            };
        }

        public static Command UpdateSchedule(string target, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter>();

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command
            {
                Name = UpdateScheduleCommand,
                Target = target,
                Parameters = parameters.ToArray()
            };
        }

        public static Command UpdateSchedule(string target)
        {
            return UpdateSchedule(target, new Dictionary<string, string>());
        }

        public static Command UpdatePlan(string target, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter>();

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command
            {
                Name = UpdatePlanCommand,
                Target = target,
                Parameters = parameters.ToArray()
            };
        }

        public static Command UpdatePlan(string target)
        {
            return UpdatePlan(target, new Dictionary<string, string>());
        }

        public static Command UpsertProduct(string target)
        {
            return UpsertProduct(target, new Dictionary<string, string>());
        }

        public static Command UpsertProduct(string target, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter>();

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));
            return new Command
            {
                Name = UpsertProductCommand,
                Target = target,
                Parameters = parameters.ToArray()
            };
        }
        public static Command UpsertPriceAdjustment(string target)
        {
            return UpsertPriceAdjustment(target, new Dictionary<string, string>());
        }

        public static Command UpsertPriceAdjustment(string target, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter>();

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));
            return new Command
            {
                Name = UpsertPriceAdjustmentCommand,
                Target = target,
                Parameters = parameters.ToArray()
            };
        }
        public static Command UpsertTrait(string target, Dictionary<string, string> dictionary)
        {
            var parameters = new List<CommandParameter>();

            parameters.AddRange(dictionary.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command { Name = UpsertTraitCommand, Target = target, Parameters = parameters.ToArray() };
        }

        public static Command UpsertUnit(string target, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter>();

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));
            return new Command { Name = UpsertUnitCommand, Target = target, Parameters = parameters.ToArray() };
        }

        public static Command UpsertIndividual(string target, Dictionary<string, string> dictionary)
        {
            var parameters = new List<CommandParameter>();

            parameters.AddRange(dictionary.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command { Name = UpsertIndividualCommand, Target = target, Parameters = parameters.ToArray() };
        }

        public static Command UpsertContract(string target)
        {
            return UpsertContract(target, new Dictionary<string, string>());
        }

        public static Command UpsertContract(string target, Dictionary<string, string> dictionary)
        {
            var parameters = new List<CommandParameter>();

            parameters.AddRange(dictionary.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command { Name = UpsertContractCommand, Target = target, Parameters = parameters.ToArray() };
        }

        public static Command SetAccountingDate(string invoiceRowHref, DateTime accountingDate)
        {
            return new Command { Name = "SetAccountingDate", Target = invoiceRowHref }.Parameter(
                "AccountingDate", accountingDate.ToShortDateString());
        }

        public static Command AddAccountingEntryLine(
            string invoiceRowHref,
            string account,
            decimal amount,
            string dimension1 = "",
            string dimension2 = "",
            string dimension3 = "",
            string dimension4 = "",
            string dimension5 = "",
            string dimension6 = "")
        {
            return new Command { Name = AddAccountingEntryLineCommand, Target = invoiceRowHref }
                .Parameter("Account", account)
                .Parameter("Amount", amount.ToString(CultureInfo.InvariantCulture))
                .Parameter("Dimension1", dimension1)
                .Parameter("Dimension2", dimension2)
                .Parameter("Dimension3", dimension3)
                .Parameter("Dimension4", dimension4)
                .Parameter("Dimension5", dimension5)
                .Parameter("Dimension6", dimension6);
        }

        public static Command UpdateInvoiceRow(string target, Dictionary<string, string> parameterMap)
        {
            var parameters = new List<CommandParameter>();

            parameters.AddRange(parameterMap.Select(pair => new CommandParameter(pair.Key, pair.Value)));

            return new Command
            {
                Name = UpdateInvoiceRowCommand,
                Target = target,
                Parameters = parameters.ToArray()
            };
        }

        public static Command UpdateLocalizationKeyword(string target, string newValue)
        {
            var parameters = new List<CommandParameter>
			{
				new CommandParameter( "Value", newValue )
			};

            return new Command
            {
                Name = UpdateLocalizationKeywordCommand,
                Target = target,
                Parameters = parameters.ToArray()
            };
        }
    }

    [Serializable]
    [XmlType(Namespace = "http://schemas.remotex.net/Apps/201207/Commands")]
    [XmlRoot(Namespace = "http://schemas.remotex.net/Apps/201207/Commands", IsNullable = false)]
    public class CommandBatch
    {
        bool continueOnErrorField;

        bool continueOnErrorFieldSpecified;

        public string Id { get; set; }

        [XmlElement("Command")]
        public Command[] Commands { get; set; }

        /// <remarks/>
        [JsonIgnore]
        [XmlElement("ContinueOnError")]
        public virtual bool ContinueOnErrorValue
        {
            get { return continueOnErrorField; }
            set { ContinueOnError = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool? ContinueOnError
        {
            get { return continueOnErrorFieldSpecified ? continueOnErrorField : new bool?(); }
            set
            {
                if (value.HasValue)
                {
                    continueOnErrorFieldSpecified = true;
                    continueOnErrorField = value.Value;
                }
                else
                    continueOnErrorFieldSpecified = false;
            }
        }

        /// <remarks/>
        [XmlIgnore]
        [JsonIgnore]
        public virtual bool ContinueOnErrorValueSpecified
        {
            get { return continueOnErrorFieldSpecified; }
            set { continueOnErrorFieldSpecified = value; }
        }

        public static CommandBatch Create(params Command[] commands)
        {
            return new CommandBatch { Commands = commands };
        }

        public static CommandBatch CreateWithContinueOnError(params Command[] commands)
        {
            return new CommandBatch { Commands = commands, ContinueOnError = true };
        }
    }

    [Serializable]
    [XmlType(Namespace = "http://schemas.remotex.net/Apps/201207/Commands")]
    [XmlRoot(Namespace = "http://schemas.remotex.net/Apps/201207/Commands", IsNullable = false)]
    public class CommandBatchResponse
    {
        DateTime _receivedField;
        bool _receivedFieldSpecified;
        bool continueOnErrorField;

        bool continueOnErrorFieldSpecified;
        public string Href { get; set; }

        public string Id { get; set; }
        public bool HasErrors { get; set; }

        /// <remarks/>
        [JsonIgnore]
        [XmlElement("Received")]
        public virtual DateTime ReceivedValue
        {
            get { return _receivedField; }
            set
            {
                _receivedFieldSpecified = true;
                _receivedField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore]
        public DateTime? Received
        {
            get { return _receivedFieldSpecified ? _receivedField : new DateTime?(); }
            set
            {
                if (value.HasValue)
                {
                    _receivedFieldSpecified = true;
                    _receivedField = value.Value;
                }
                else
                    _receivedFieldSpecified = false;
            }
        }

        /// <remarks/>
        [XmlIgnore]
        [JsonIgnore]
        public virtual bool ReceivedValueSpecified
        {
            get { return _receivedFieldSpecified; }
            set { _receivedFieldSpecified = value; }
        }

        [XmlElement("CommandResponse")]
        public CommandResponse[] Commands { get; set; }

        /// <remarks/>
        [JsonIgnore]
        [XmlElement("ContinueOnError")]
        public virtual bool ContinueOnErrorValue
        {
            get { return continueOnErrorField; }
            set { ContinueOnError = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool? ContinueOnError
        {
            get { return continueOnErrorFieldSpecified ? continueOnErrorField : new bool?(); }
            set
            {
                if (value.HasValue)
                {
                    continueOnErrorFieldSpecified = true;
                    continueOnErrorField = value.Value;
                }
                else
                    continueOnErrorFieldSpecified = false;
            }
        }

        /// <remarks/>
        [XmlIgnore]
        [JsonIgnore]
        public virtual bool ContinueOnErrorValueSpecified
        {
            get { return continueOnErrorFieldSpecified; }
            set { continueOnErrorFieldSpecified = value; }
        }
    }

    [Serializable]
    [XmlType(Namespace = "http://schemas.remotex.net/Apps/201207/Commands")]
    public class Command
    {
        static readonly SHA1 Sha1 = SHA1.Create();

        public string Name { get; set; }
        public string Target { get; set; }

        [XmlElement("Parameter")]
        public CommandParameter[] Parameters { get; set; }

        public byte[] ComputeHash()
        {
            var sb = new StringBuilder(Name.ToUpperInvariant());
            sb.AppendLine(Target ?? string.Empty);
            if (null != Parameters)
                foreach (var parameter in
                    (from p in Parameters let name = p.Name.ToLowerInvariant() orderby name select new { name, p.Values }))
                    sb.AppendLine(string.Concat(parameter.name, "=", string.Join("|", parameter.Values)));
            return Sha1.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
        }

        public string SHA1String()
        {
            return BitConverter.ToString(ComputeHash()).Replace("-", "");
        }

        public string[] ParameterValues(string name)
        {
            if (Parameters == null)
                return null;
            CommandParameter parameter =
                Parameters.FirstOrDefault(p => String.Compare(p.Name, name, StringComparison.InvariantCultureIgnoreCase) == 0);
            return parameter != null ? parameter.Values : null;
        }

        public string ParameterValue(string name)
        {
            string[] vals = ParameterValues(name);
            return null != vals ? vals.FirstOrDefault() : null;
        }

        public Command Parameter(string parameterName, string parameterValue)
        {
            return Parameter(parameterName, new[] { parameterValue });
        }

        public Command Parameter(string parameterName, params string[] parameterValue)
        {
            CommandParameter[] parameterValues = Parameters ?? new CommandParameter[0];
            CommandParameter parameter = parameterValues.FirstOrDefault(p => p.Name == parameterName);
            if (parameter == null)
                Parameters =
                    new List<CommandParameter>(parameterValues) { new CommandParameter(parameterName, parameterValue) }.ToArray();
            else
                parameter.Values = parameterValue;
            return this;
        }
    }

    [Serializable]
    [XmlType(Namespace = "http://schemas.remotex.net/Apps/201207/Commands")]
    public class CommandResponse
    {
        DateTime _occurredAtField;
        bool _occurredAtFieldSpecified;

        public string ErrorMessage { get; set; }
        public bool HasErrors { get; set; }

        /// <remarks/>
        [JsonIgnore]
        [XmlElement("OccurredAt")]
        public virtual DateTime OccurredAtValue
        {
            get { return _occurredAtField; }
            set
            {
                _occurredAtFieldSpecified = true;
                _occurredAtField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore]
        public DateTime? OccurredAt
        {
            get { return _occurredAtFieldSpecified ? _occurredAtField : new DateTime?(); }
            set
            {
                if (value.HasValue)
                {
                    _occurredAtFieldSpecified = true;
                    _occurredAtField = value.Value;
                }
                else
                    _occurredAtFieldSpecified = false;
            }
        }

        /// <remarks/>
        [XmlIgnore]
        [JsonIgnore]
        public virtual bool OccurredAtValueSpecified
        {
            get { return _occurredAtFieldSpecified; }
            set { _occurredAtFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlArray("AffectedItems")]
        [XmlArrayItem("Item")]
        [JsonProperty("AffectedItems")]
        public AffectedItem[] AffectedItems { get; set; }

        public Command Command { get; set; }

        public CommandResponse Fail(string errorMessage)
        {
            AffectedItems = new AffectedItem[0];
            HasErrors = true;
            ErrorMessage = errorMessage;
            return this;
        }
    }

    [Serializable]
    [XmlType(Namespace = "http://schemas.remotex.net/Apps/201207/Commands")]
    public class CommandParameter
    {
        public CommandParameter() { }

        public CommandParameter(string name, params string[] value)
        {
            Name = name;
            Values = value;
        }

        public string Name { get; set; }

        [XmlElement("Value")]
        public string[] Values { get; set; }

        /// <summary>
        /// If values array have a length of 1, that value is returned, otherwise, null.
        /// </summary>
        public string Value
        {
            get { return (null != Values && Values.Length == 1) ? Values[0] : null; }
        }
    }

    [Serializable]
    [XmlType(Namespace = "http://schemas.remotex.net/Apps/201207/Commands")]
    public class AffectedItem
    {
        public string Href { get; set; }
        public string Revision { get; set; }
    }
}