using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using doanC_.Services.Localization;

namespace doanC_.ViewModels
{
    public class QrScannerViewModel
    {
        public string QrScannerTitle => AppResources.GetString("QrScannerTitle");
        public string QrScannerSubtitle => AppResources.GetString("QrScannerSubtitle");
        public string QrInstructions => AppResources.GetString("QrInstructions");
        public string QrManualInput => AppResources.GetString("QrManualInput");
    }
}
