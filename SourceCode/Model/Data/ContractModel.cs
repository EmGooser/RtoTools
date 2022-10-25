using RtoTools.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtoTools.Model.Data
{
    public class ContractModel
    {
        /// <summary>
        /// A unique identifier used for processing in this application
        /// </summary>
        public int ContractId { get; set; }

        /// <summary>
        /// The faction to impersonate, only for contract types that allow you to impersonate another faction 
        /// </summary>
        public int? AsFactionId { get; set; }

        /// <summary>
        /// The type of contract
        /// </summary>
        public ContractType ContractType { get; set; }

        /// <summary>
        /// The star system to open the contract on
        /// </summary>
        public int SystemId { get; set; }

        /// <summary>
        /// The target level of control required, valid range of 1 - 300, only used for Fortify contract types
        /// </summary>
        public double? TargetControl { get; set; }

        /// <summary>
        /// The faction being targeted (opFor) of the contract, used by Attack,  DeniableAsset, FalseFlag contract types
        /// </summary>
        public int? TargetFactionId { get; set; }
    }
}