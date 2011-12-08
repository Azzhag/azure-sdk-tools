// ----------------------------------------------------------------------------------
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureDeploymentCmdlets.Model
{
    public class ArgumentConstants
    {
        public static Dictionary<Location, string> Locations { get; private set; }
        public static Dictionary<Slot, string> Slots { get; private set; }

        static ArgumentConstants()
        {
            Locations = new Dictionary<Location, string>()
            {
                { Location.AnywhereAsia, "anywhere asia" },
                { Location.AnywhereEurope, "anywhere europe" },
                { Location.AnywhereUS, "anywhere us" },
                { Location.EastAsia, "east asia" },
                { Location.NorthCentralUS, "north central us" },
                { Location.NorthEurope, "north europe" },
                { Location.SouthCentralUS, "south central us" },
                { Location.SouthEastAsia, "southeast asia" },
                { Location.WestEurope, "west europe" },
            };

            Slots = new Dictionary<Slot, string>()
            {
                { Slot.Production, "production" },
                { Slot.Staging, "staging" }
            };
        }
    }

    public enum Location
    {
        NorthCentralUS,
        AnywhereUS,
        AnywhereEurope,
        WestEurope,
        SouthCentralUS,
        NorthEurope,
        AnywhereAsia,
        SouthEastAsia,
        EastAsia
    }
    
    public enum Slot
    {
        Production,
        Staging
    }
    
    public enum DevEnv
    {
        Local,
        Cloud
    }

    public enum RoleType
    {
        WebRole,
        WorkerRole
    }
}