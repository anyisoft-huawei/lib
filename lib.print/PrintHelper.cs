using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;

namespace lib.print
{
    public  class PrintHelper 
    {

        public PrintHelper()
        {
          
        }

        public static List<string> GetPrinterNames()
        {
            List<string> ls = new List<string>();
            foreach (string i in PrinterSettings.InstalledPrinters)
            {
                ls.Add(i);
            }
            return ls;
        }
        
        /// <summary>
        /// 获取指定打印机支持的纸张名称集合
        /// </summary>
        /// <param name="PS"></param>
        /// <returns></returns>
        public static List<string> GetPaperNamesForPrinter(PrinterSettings PS)
        {
            List<string> ls = new List<string>();

            foreach (PaperSize i in PS.PaperSizes)
            {
                ls.Add(i.PaperName);
            }
            return ls;
        }

        /// <summary>
        /// 获取全部纸张
        /// </summary>
        /// <returns></returns>
        public static List<string> GetPaperNames()
        {
            List<string> ls = new List<string>();
            foreach (string i in Enum.GetNames(typeof(PaperKind)))
            {
                 ls.Add(i);
            }
            return ls;
        }

        public static PaperSize getPaper(string Name)
        {
            PaperSize pSize = new PaperSize();
            switch (Name)
            {
                #region 纸张
                case "A2":
                    pSize.PaperName = "A2";
                    pSize.RawKind = (int)PaperKind.A2;
                    pSize.Width = PrinterUnitConvert.Convert(4200, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(5940, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "A3":
                    pSize.PaperName = "A3";
                    pSize.RawKind = (int)PaperKind.A3;
                    pSize.Width = PrinterUnitConvert.Convert(2970, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(4200, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "A3Extra":
                    pSize.PaperName = "A3Extra";
                    pSize.RawKind = (int)PaperKind.A3Extra;
                    pSize.Width = PrinterUnitConvert.Convert(3220, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(4450, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "A3ExtraTransverse":
                    pSize.PaperName = "A3ExtraTransverse";
                    pSize.RawKind = (int)PaperKind.A3ExtraTransverse;
                    pSize.Width = PrinterUnitConvert.Convert(3220, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(4450, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "A3Rotated":
                    pSize.PaperName = "A3Rotated";
                    pSize.RawKind = (int)PaperKind.A3Rotated;
                    pSize.Width = PrinterUnitConvert.Convert(4200, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2970, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "A3Transverse":
                    pSize.PaperName = "A3Transverse";
                    pSize.RawKind = (int)PaperKind.A3Transverse;
                    pSize.Width = PrinterUnitConvert.Convert(2970, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(4200, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "A4":
                    pSize.PaperName = "A4";
                    pSize.RawKind = (int)PaperKind.A4;
                    pSize.Width = PrinterUnitConvert.Convert(2100, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2970, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "A4Extra":
                    pSize.PaperName = "A4Extra";
                    pSize.RawKind = (int)PaperKind.A4Extra;
                    pSize.Width = PrinterUnitConvert.Convert(2360, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(3220, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "A4Plus":
                    pSize.PaperName = "A4Plus";
                    pSize.RawKind = (int)PaperKind.A4Plus;
                    pSize.Width = PrinterUnitConvert.Convert(2100, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(3300, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "A4Rotated":
                    pSize.PaperName = "A4Rotated";
                    pSize.RawKind = (int)PaperKind.A4Rotated;
                    pSize.Width = PrinterUnitConvert.Convert(2970, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2100, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "A4Small":
                    pSize.PaperName = "A4Small";
                    pSize.RawKind = (int)PaperKind.A4Small;
                    pSize.Width = PrinterUnitConvert.Convert(2100, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2970, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "A4Transverse":
                    pSize.PaperName = "A4Transverse";
                    pSize.RawKind = (int)PaperKind.A4Transverse;
                    pSize.Width = PrinterUnitConvert.Convert(2100, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2970, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "A5":
                    pSize.PaperName = "A5";
                    pSize.RawKind = (int)PaperKind.A5;
                    pSize.Width = PrinterUnitConvert.Convert(1480, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2100, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "A5Extra":
                    pSize.PaperName = "A5Extra";
                    pSize.RawKind = (int)PaperKind.A5Extra;
                    pSize.Width = PrinterUnitConvert.Convert(1740, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2350, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "A5Rotated":
                    pSize.PaperName = "A5Rotated";
                    pSize.RawKind = (int)PaperKind.A5Rotated;
                    pSize.Width = PrinterUnitConvert.Convert(2100, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(1480, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "A5Transverse":
                    pSize.PaperName = "A5Transverse";
                    pSize.RawKind = (int)PaperKind.A5Transverse;
                    pSize.Width = PrinterUnitConvert.Convert(1480, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2100, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "A6":
                    pSize.PaperName = "A6";
                    pSize.RawKind = (int)PaperKind.A6;
                    pSize.Width = PrinterUnitConvert.Convert(1050, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(1480, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "A6Rotated":
                    pSize.PaperName = "A6Rotated";
                    pSize.RawKind = (int)PaperKind.A6Rotated;
                    pSize.Width = PrinterUnitConvert.Convert(1480, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(1050, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "APlus":
                    pSize.PaperName = "APlus";
                    pSize.RawKind = (int)PaperKind.APlus;
                    pSize.Width = PrinterUnitConvert.Convert(2270, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(3560, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "B4":
                    pSize.PaperName = "B4";
                    pSize.RawKind = (int)PaperKind.B4;
                    pSize.Width = PrinterUnitConvert.Convert(2500, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(3530, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "B4Envelope":
                    pSize.PaperName = "B4Envelope";
                    pSize.RawKind = (int)PaperKind.B4Envelope;
                    pSize.Width = PrinterUnitConvert.Convert(2500, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(3530, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "B4JisRotated":
                    pSize.PaperName = "B4JisRotated";
                    pSize.RawKind = (int)PaperKind.B4JisRotated;
                    pSize.Width = PrinterUnitConvert.Convert(3640, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2570, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "B5":
                    pSize.PaperName = "B5";
                    pSize.RawKind = (int)PaperKind.B5;
                    pSize.Width = PrinterUnitConvert.Convert(1760, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2500, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "B5Envelope":
                    pSize.PaperName = "B5Envelope";
                    pSize.RawKind = (int)PaperKind.B5Envelope;
                    pSize.Width = PrinterUnitConvert.Convert(1760, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2500, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "B5Extra":
                    pSize.PaperName = "B5Extra";
                    pSize.RawKind = (int)PaperKind.B5Extra;
                    pSize.Width = PrinterUnitConvert.Convert(2010, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2760, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "B5JisRotated":
                    pSize.PaperName = "B5JisRotated";
                    pSize.RawKind = (int)PaperKind.B5JisRotated;
                    pSize.Width = PrinterUnitConvert.Convert(2570, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(1820, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "B5Transverse":
                    pSize.PaperName = "B5Transverse";
                    pSize.RawKind = (int)PaperKind.B5Transverse;
                    pSize.Width = PrinterUnitConvert.Convert(1820, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2570, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "B6Envelope":
                    pSize.PaperName = "B6Envelope";
                    pSize.RawKind = (int)PaperKind.B6Envelope;
                    pSize.Width = PrinterUnitConvert.Convert(1760, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(1250, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "B6Jis":
                    pSize.PaperName = "B6Jis";
                    pSize.RawKind = (int)PaperKind.B6Jis;
                    pSize.Width = PrinterUnitConvert.Convert(1280, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(1820, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "B6JisRotated":
                    pSize.PaperName = "B6JisRotated";
                    pSize.RawKind = (int)PaperKind.B6JisRotated;
                    pSize.Width = PrinterUnitConvert.Convert(1820, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(1280, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "BPlus":
                    pSize.PaperName = "BPlus";
                    pSize.RawKind = (int)PaperKind.BPlus;
                    pSize.Width = PrinterUnitConvert.Convert(3050, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(4870, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "C3Envelope":
                    pSize.PaperName = "C3Envelope";
                    pSize.RawKind = (int)PaperKind.C3Envelope;
                    pSize.Width = PrinterUnitConvert.Convert(3240, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(4580, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "C4Envelope":
                    pSize.PaperName = "C4Envelope";
                    pSize.RawKind = (int)PaperKind.C4Envelope;
                    pSize.Width = PrinterUnitConvert.Convert(2290, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(3240, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "C5Envelope":
                    pSize.PaperName = "C5Envelope";
                    pSize.RawKind = (int)PaperKind.C5Envelope;
                    pSize.Width = PrinterUnitConvert.Convert(1620, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2290, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "C65Envelope":
                    pSize.PaperName = "C65Envelope";
                    pSize.RawKind = (int)PaperKind.C65Envelope;
                    pSize.Width = PrinterUnitConvert.Convert(1140, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2290, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "C6Envelope":
                    pSize.PaperName = "C6Envelope";
                    pSize.RawKind = (int)PaperKind.C6Envelope;
                    pSize.Width = PrinterUnitConvert.Convert(1140, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(1620, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "CSheet":
                    pSize.PaperName = "CSheet";
                    pSize.RawKind = (int)PaperKind.CSheet;
                    pSize.Width = 1700;
                    pSize.Height = 2200;
                    break;
                case "Custom":
                    pSize.PaperName = "Custom";
                    pSize.RawKind = (int)PaperKind.Custom;
                    ;
                    ;
                    break;
                case "DLEnvelope":
                    pSize.PaperName = "DLEnvelope";
                    pSize.RawKind = (int)PaperKind.DLEnvelope;
                    pSize.Width = PrinterUnitConvert.Convert(1100, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2200, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "DSheet":
                    pSize.PaperName = "DSheet";
                    pSize.RawKind = (int)PaperKind.DSheet;
                    pSize.Width = 2200;
                    pSize.Height = 3400;
                    break;
                case "ESheet":
                    pSize.PaperName = "ESheet";
                    pSize.RawKind = (int)PaperKind.ESheet;
                    pSize.Width = 3400;
                    pSize.Height = 4400;
                    break;
                case "Executive":
                    pSize.PaperName = "Executive";
                    pSize.RawKind = (int)PaperKind.Executive;
                    pSize.Width = 725;
                    pSize.Height = 1050;
                    break;
                case "Folio":
                    pSize.PaperName = "Folio";
                    pSize.RawKind = (int)PaperKind.Folio;
                    pSize.Width = 850;
                    pSize.Height = 1300;
                    break;
                case "GermanLegalFanfold":
                    pSize.PaperName = "GermanLegalFanfold";
                    pSize.RawKind = (int)PaperKind.GermanLegalFanfold;
                    pSize.Width = 850;
                    pSize.Height = 1300;
                    break;
                case "GermanStandardFanfold":
                    pSize.PaperName = "GermanStandardFanfold";
                    pSize.RawKind = (int)PaperKind.GermanStandardFanfold;
                    pSize.Width = 850;
                    pSize.Height = 1200;
                    break;
                case "InviteEnvelope":
                    pSize.PaperName = "InviteEnvelope";
                    pSize.RawKind = (int)PaperKind.InviteEnvelope;
                    pSize.Width = PrinterUnitConvert.Convert(2200, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2200, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "IsoB4":
                    pSize.PaperName = "IsoB4";
                    pSize.RawKind = (int)PaperKind.IsoB4;
                    pSize.Width = PrinterUnitConvert.Convert(2500, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(3530, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "ItalyEnvelope":
                    pSize.PaperName = "ItalyEnvelope";
                    pSize.RawKind = (int)PaperKind.ItalyEnvelope;
                    pSize.Width = PrinterUnitConvert.Convert(1100, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2300, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "JapaneseDoublePostcard":
                    pSize.PaperName = "JapaneseDoublePostcard";
                    pSize.RawKind = (int)PaperKind.JapaneseDoublePostcard;
                    pSize.Width = PrinterUnitConvert.Convert(2000, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(1480, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "JapaneseDoublePostcardRotated":
                    pSize.PaperName = "JapaneseDoublePostcardRotated";
                    pSize.RawKind = (int)PaperKind.JapaneseDoublePostcardRotated;
                    pSize.Width = PrinterUnitConvert.Convert(1480, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2000, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "JapaneseEnvelopeChouNumber3":
                    pSize.PaperName = "JapaneseEnvelopeChouNumber3";
                    pSize.RawKind = (int)PaperKind.JapaneseEnvelopeChouNumber3;
                    ;
                    ;
                    break;
                case "JapaneseEnvelopeChouNumber3Rotated":
                    pSize.PaperName = "JapaneseEnvelopeChouNumber3Rotated";
                    pSize.RawKind = (int)PaperKind.JapaneseEnvelopeChouNumber3Rotated;
                    ;
                    ;
                    break;
                case "JapaneseEnvelopeChouNumber4":
                    pSize.PaperName = "JapaneseEnvelopeChouNumber4";
                    pSize.RawKind = (int)PaperKind.JapaneseEnvelopeChouNumber4;
                    ;
                    ;
                    break;
                case "JapaneseEnvelopeChouNumber4Rotated":
                    pSize.PaperName = "JapaneseEnvelopeChouNumber4Rotated";
                    pSize.RawKind = (int)PaperKind.JapaneseEnvelopeChouNumber4Rotated;
                    ;
                    ;
                    break;
                case "JapaneseEnvelopeKakuNumber2":
                    pSize.PaperName = "JapaneseEnvelopeKakuNumber2";
                    pSize.RawKind = (int)PaperKind.JapaneseEnvelopeKakuNumber2;
                    ;
                    ;
                    break;
                case "JapaneseEnvelopeKakuNumber2Rotated":
                    pSize.PaperName = "JapaneseEnvelopeKakuNumber2Rotated";
                    pSize.RawKind = (int)PaperKind.JapaneseEnvelopeKakuNumber2Rotated;
                    ;
                    ;
                    break;
                case "JapaneseEnvelopeKakuNumber3":
                    pSize.PaperName = "JapaneseEnvelopeKakuNumber3";
                    pSize.RawKind = (int)PaperKind.JapaneseEnvelopeKakuNumber3;
                    ;
                    ;
                    break;
                case "JapaneseEnvelopeKakuNumber3Rotated":
                    pSize.PaperName = "JapaneseEnvelopeKakuNumber3Rotated";
                    pSize.RawKind = (int)PaperKind.JapaneseEnvelopeKakuNumber3Rotated;
                    ;
                    ;
                    break;
                case "JapaneseEnvelopeYouNumber4":
                    pSize.PaperName = "JapaneseEnvelopeYouNumber4";
                    pSize.RawKind = (int)PaperKind.JapaneseEnvelopeYouNumber4;
                    ;
                    ;
                    break;
                case "JapaneseEnvelopeYouNumber4Rotated":
                    pSize.PaperName = "JapaneseEnvelopeYouNumber4Rotated";
                    pSize.RawKind = (int)PaperKind.JapaneseEnvelopeYouNumber4Rotated;
                    ;
                    ;
                    break;
                case "JapanesePostcard":
                    pSize.PaperName = "JapanesePostcard";
                    pSize.RawKind = (int)PaperKind.JapanesePostcard;
                    pSize.Width = PrinterUnitConvert.Convert(1000, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(1480, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "JapanesePostcardRotated":
                    pSize.PaperName = "JapanesePostcardRotated";
                    pSize.RawKind = (int)PaperKind.JapanesePostcardRotated;
                    pSize.Width = PrinterUnitConvert.Convert(1480, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(1000, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "Ledger":
                    pSize.PaperName = "Ledger";
                    pSize.RawKind = (int)PaperKind.Ledger;
                    pSize.Width = 1700;
                    pSize.Height = 1100;
                    break;
                case "Legal":
                    pSize.PaperName = "Legal";
                    pSize.RawKind = (int)PaperKind.Legal;
                    pSize.Width = 850;
                    pSize.Height = 1400;
                    break;
                case "LegalExtra":
                    pSize.PaperName = "LegalExtra";
                    pSize.RawKind = (int)PaperKind.LegalExtra;
                    pSize.Width = (int)927.5;
                    pSize.Height = 1500;
                    break;
                case "Letter":
                    pSize.PaperName = "Letter";
                    pSize.RawKind = (int)PaperKind.Letter;
                    pSize.Width = 850;
                    pSize.Height = 1100;
                    break;
                case "LetterExtra":
                    pSize.PaperName = "LetterExtra";
                    pSize.RawKind = (int)PaperKind.LetterExtra;
                    pSize.Width = (int)927.5;
                    pSize.Height = 1200;
                    break;
                case "LetterExtraTransverse":
                    pSize.PaperName = "LetterExtraTransverse";
                    pSize.RawKind = (int)PaperKind.LetterExtraTransverse;
                    pSize.Width = (int)927.5;
                    pSize.Height = 1200;
                    break;
                case "LetterPlus":
                    pSize.PaperName = "LetterPlus";
                    pSize.RawKind = (int)PaperKind.LetterPlus;
                    pSize.Width = 850;
                    pSize.Height = 1269;
                    break;
                case "LetterRotated":
                    pSize.PaperName = "LetterRotated";
                    pSize.RawKind = (int)PaperKind.LetterRotated;
                    pSize.Width = 1100;
                    pSize.Height = 850;
                    break;
                case "LetterSmall":
                    pSize.PaperName = "LetterSmall";
                    pSize.RawKind = (int)PaperKind.LetterSmall;
                    pSize.Width = 850;
                    pSize.Height = 1100;
                    break;
                case "LetterTransverse":
                    pSize.PaperName = "LetterTransverse";
                    pSize.RawKind = (int)PaperKind.LetterTransverse;
                    pSize.Width = (int)827.5;
                    pSize.Height = 1100;
                    break;
                case "MonarchEnvelope":
                    pSize.PaperName = "MonarchEnvelope";
                    pSize.RawKind = (int)PaperKind.MonarchEnvelope;
                    pSize.Width = (int)387.5;
                    pSize.Height = 750;
                    break;
                case "Note":
                    pSize.PaperName = "Note";
                    pSize.RawKind = (int)PaperKind.Note;
                    pSize.Width = 850;
                    pSize.Height = 1100;
                    break;
                case "Number10Envelope":
                    pSize.PaperName = "Number10Envelope";
                    pSize.RawKind = (int)PaperKind.Number10Envelope;
                    pSize.Width = (int)412.5;
                    pSize.Height = 950;
                    break;
                case "Number11Envelope":
                    pSize.PaperName = "Number11Envelope";
                    pSize.RawKind = (int)PaperKind.Number11Envelope;
                    pSize.Width = 450;
                    pSize.Height = (int)1037.5;
                    break;
                case "Number12Envelope":
                    pSize.PaperName = "Number12Envelope";
                    pSize.RawKind = (int)PaperKind.Number12Envelope;
                    pSize.Width = 475;
                    pSize.Height = 1100;
                    break;
                case "Number14Envelope":
                    pSize.PaperName = "Number14Envelope";
                    pSize.RawKind = (int)PaperKind.Number14Envelope;
                    pSize.Width = 500;
                    pSize.Height = 1150;
                    break;
                case "Number9Envelope":
                    pSize.PaperName = "Number9Envelope";
                    pSize.RawKind = (int)PaperKind.Number9Envelope;
                    pSize.Width = (int)387.5;
                    pSize.Height = (int)887.5;
                    break;
                case "PersonalEnvelope":
                    pSize.PaperName = "PersonalEnvelope";
                    pSize.RawKind = (int)PaperKind.PersonalEnvelope;
                    pSize.Width = (int)362.5;
                    pSize.Height = 650;
                    break;
                case "Prc16K":
                    pSize.PaperName = "Prc16K";
                    pSize.RawKind = (int)PaperKind.Prc16K;
                    pSize.Width = PrinterUnitConvert.Convert(1460, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2150, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "Prc16KRotated":
                    pSize.PaperName = "Prc16KRotated";
                    pSize.RawKind = (int)PaperKind.Prc16KRotated;
                    pSize.Width = PrinterUnitConvert.Convert(1460, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2150, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "Prc32K":
                    pSize.PaperName = "Prc32K";
                    pSize.RawKind = (int)PaperKind.Prc32K;
                    pSize.Width = PrinterUnitConvert.Convert(970, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(1510, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "Prc32KBig":
                    pSize.PaperName = "Prc32KBig";
                    pSize.RawKind = (int)PaperKind.Prc32KBig;
                    pSize.Width = PrinterUnitConvert.Convert(970, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(1510, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "Prc32KBigRotated":
                    pSize.PaperName = "Prc32KBigRotated";
                    pSize.RawKind = (int)PaperKind.Prc32KBigRotated;
                    pSize.Width = PrinterUnitConvert.Convert(970, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(1510, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "Prc32KRotated":
                    pSize.PaperName = "Prc32KRotated";
                    pSize.RawKind = (int)PaperKind.Prc32KRotated;
                    pSize.Width = PrinterUnitConvert.Convert(970, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(1510, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "PrcEnvelopeNumber1":
                    pSize.PaperName = "PrcEnvelopeNumber1";
                    pSize.RawKind = (int)PaperKind.PrcEnvelopeNumber1;
                    pSize.Width = PrinterUnitConvert.Convert(1020, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(1650, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "PrcEnvelopeNumber10":
                    pSize.PaperName = "PrcEnvelopeNumber10";
                    pSize.RawKind = (int)PaperKind.PrcEnvelopeNumber10;
                    pSize.Width = PrinterUnitConvert.Convert(3240, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(4580, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "PrcEnvelopeNumber10Rotated":
                    pSize.PaperName = "PrcEnvelopeNumber10Rotated";
                    pSize.RawKind = (int)PaperKind.PrcEnvelopeNumber10Rotated;
                    pSize.Width = PrinterUnitConvert.Convert(4580, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(3240, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "PrcEnvelopeNumber1Rotated":
                    pSize.PaperName = "PrcEnvelopeNumber1Rotated";
                    pSize.RawKind = (int)PaperKind.PrcEnvelopeNumber1Rotated;
                    pSize.Width = PrinterUnitConvert.Convert(1650, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(1020, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "PrcEnvelopeNumber2":
                    pSize.PaperName = "PrcEnvelopeNumber2";
                    pSize.RawKind = (int)PaperKind.PrcEnvelopeNumber2;
                    pSize.Width = PrinterUnitConvert.Convert(1020, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(1760, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "PrcEnvelopeNumber2Rotated":
                    pSize.PaperName = "PrcEnvelopeNumber2Rotated";
                    pSize.RawKind = (int)PaperKind.PrcEnvelopeNumber2Rotated;
                    pSize.Width = PrinterUnitConvert.Convert(1760, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(1020, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "PrcEnvelopeNumber3":
                    pSize.PaperName = "PrcEnvelopeNumber3";
                    pSize.RawKind = (int)PaperKind.PrcEnvelopeNumber3;
                    pSize.Width = PrinterUnitConvert.Convert(1250, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(1760, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "PrcEnvelopeNumber3Rotated":
                    pSize.PaperName = "PrcEnvelopeNumber3Rotated";
                    pSize.RawKind = (int)PaperKind.PrcEnvelopeNumber3Rotated;
                    pSize.Width = PrinterUnitConvert.Convert(1760, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(1250, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "PrcEnvelopeNumber4":
                    pSize.PaperName = "PrcEnvelopeNumber4";
                    pSize.RawKind = (int)PaperKind.PrcEnvelopeNumber4;
                    pSize.Width = PrinterUnitConvert.Convert(1100, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2080, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "PrcEnvelopeNumber4Rotated":
                    pSize.PaperName = "PrcEnvelopeNumber4Rotated";
                    pSize.RawKind = (int)PaperKind.PrcEnvelopeNumber4Rotated;
                    pSize.Width = PrinterUnitConvert.Convert(2080, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(1100, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "PrcEnvelopeNumber5":
                    pSize.PaperName = "PrcEnvelopeNumber5";
                    pSize.RawKind = (int)PaperKind.PrcEnvelopeNumber5;
                    pSize.Width = PrinterUnitConvert.Convert(1100, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2200, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "PrcEnvelopeNumber5Rotated":
                    pSize.PaperName = "PrcEnvelopeNumber5Rotated";
                    pSize.RawKind = (int)PaperKind.PrcEnvelopeNumber5Rotated;
                    pSize.Width = PrinterUnitConvert.Convert(2200, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(1100, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "PrcEnvelopeNumber6":
                    pSize.PaperName = "PrcEnvelopeNumber6";
                    pSize.RawKind = (int)PaperKind.PrcEnvelopeNumber6;
                    pSize.Width = PrinterUnitConvert.Convert(1200, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2300, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "PrcEnvelopeNumber6Rotated":
                    pSize.PaperName = "PrcEnvelopeNumber6Rotated";
                    pSize.RawKind = (int)PaperKind.PrcEnvelopeNumber6Rotated;
                    pSize.Width = PrinterUnitConvert.Convert(2300, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(1200, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "PrcEnvelopeNumber7":
                    pSize.PaperName = "PrcEnvelopeNumber7";
                    pSize.RawKind = (int)PaperKind.PrcEnvelopeNumber7;
                    pSize.Width = PrinterUnitConvert.Convert(1600, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2300, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "PrcEnvelopeNumber7Rotated":
                    pSize.PaperName = "PrcEnvelopeNumber7Rotated";
                    pSize.RawKind = (int)PaperKind.PrcEnvelopeNumber7Rotated;
                    pSize.Width = PrinterUnitConvert.Convert(2300, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(1600, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "PrcEnvelopeNumber8":
                    pSize.PaperName = "PrcEnvelopeNumber8";
                    pSize.RawKind = (int)PaperKind.PrcEnvelopeNumber8;
                    pSize.Width = PrinterUnitConvert.Convert(1200, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(3090, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "PrcEnvelopeNumber8Rotated":
                    pSize.PaperName = "PrcEnvelopeNumber8Rotated";
                    pSize.RawKind = (int)PaperKind.PrcEnvelopeNumber8Rotated;
                    pSize.Width = PrinterUnitConvert.Convert(3090, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(1200, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "PrcEnvelopeNumber9":
                    pSize.PaperName = "PrcEnvelopeNumber9";
                    pSize.RawKind = (int)PaperKind.PrcEnvelopeNumber9;
                    pSize.Width = PrinterUnitConvert.Convert(2290, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(3240, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "PrcEnvelopeNumber9Rotated":
                    pSize.PaperName = "PrcEnvelopeNumber9Rotated";
                    pSize.RawKind = (int)PaperKind.PrcEnvelopeNumber9Rotated;
                    pSize.Width = PrinterUnitConvert.Convert(3240, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2290, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "Quarto":
                    pSize.PaperName = "Quarto";
                    pSize.RawKind = (int)PaperKind.Quarto;
                    pSize.Width = PrinterUnitConvert.Convert(2150, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2750, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
                case "Standard10x11":
                    pSize.PaperName = "Standard10x11";
                    pSize.RawKind = (int)PaperKind.Standard10x11;
                    pSize.Width = 1000;
                    pSize.Height = 1100;
                    break;
                case "Standard10x14":
                    pSize.PaperName = "Standard10x14";
                    pSize.RawKind = (int)PaperKind.Standard10x14;
                    pSize.Width = 1000;
                    pSize.Height = 1400;
                    break;
                case "Standard11x17":
                    pSize.PaperName = "Standard11x17";
                    pSize.RawKind = (int)PaperKind.Standard11x17;
                    pSize.Width = 1100;
                    pSize.Height = 1700;
                    break;
                case "Standard12x11":
                    pSize.PaperName = "Standard12x11";
                    pSize.RawKind = (int)PaperKind.Standard12x11;
                    pSize.Width = 1200;
                    pSize.Height = 1100;
                    break;
                case "Standard15x11":
                    pSize.PaperName = "Standard15x11";
                    pSize.RawKind = (int)PaperKind.Standard15x11;
                    pSize.Width = 1500;
                    pSize.Height = 1100;
                    break;
                case "Standard9x11":
                    pSize.PaperName = "Standard9x11";
                    pSize.RawKind = (int)PaperKind.Standard9x11;
                    pSize.Width = 900;
                    pSize.Height = 1100;
                    break;
                case "Statement":
                    pSize.PaperName = "Statement";
                    pSize.RawKind = (int)PaperKind.Statement;
                    pSize.Width = 550;
                    pSize.Height = 850;
                    break;
                case "Tabloid":
                    pSize.PaperName = "Tabloid";
                    pSize.RawKind = (int)PaperKind.Tabloid;
                    pSize.Width = 1100;
                    pSize.Height = 1700;
                    break;
                case "TabloidExtra":
                    pSize.PaperName = "TabloidExtra";
                    pSize.RawKind = (int)PaperKind.TabloidExtra;
                    pSize.Width = 1169;
                    pSize.Height = 1800;
                    break;
                case "USStandardFanfold":
                    pSize.PaperName = "USStandardFanfold";
                    pSize.RawKind = (int)PaperKind.USStandardFanfold;
                    pSize.Width = (int)1487.5;
                    pSize.Height = 1100;
                    break;

                #endregion
                default:
                    pSize.PaperName = "A4";
                    pSize.RawKind = (int)PaperKind.A4;
                    pSize.Width = PrinterUnitConvert.Convert(2100, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    pSize.Height = PrinterUnitConvert.Convert(2970, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
                    break;
            }
            return pSize;
        }

        /// <summary>
        /// 获取指定打印机的默认纸张
        /// </summary>
        /// <param name="PS"></param>
        /// <returns></returns>
        public static string GetDefaultPaperName(PrinterSettings PS)
        {
            return PS.DefaultPageSettings.PaperSize.PaperName;
        }


        /// <summary>
        /// 查找指定纸张
        /// </summary>
        /// <param name="PaperName"></param>
        /// <param name="PS"></param>
        /// <returns></returns>
        public static PaperSize GetPaperSize(string PaperName, PrinterSettings PS)
        {
            foreach (PaperSize i in PS.PaperSizes)
            {
                if (i.PaperName == PaperName) return i;
            }
            throw new Exception("未找到指定纸张");
        }
        
        /// <summary>
        /// 像素转毫米
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static int PixelToMm(int p)
        {
            int i = PrinterUnitConvert.Convert(p, PrinterUnit.Display, PrinterUnit.TenthsOfAMillimeter);
            return i / 10 + (i % 10 > 4 ? 1 : 0);
        }
        /// <summary>
        /// 像素转毫米
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static float PixelToMm(float p)
        {
            return ((float)PrinterUnitConvert.Convert(p, PrinterUnit.Display, PrinterUnit.TenthsOfAMillimeter)) / 10;
        }
        /// <summary>
        /// 像素转毫米
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static double PixelToMm(double p)
        {
            return ((float)PrinterUnitConvert.Convert(p, PrinterUnit.Display, PrinterUnit.TenthsOfAMillimeter)) / 10;
        }

        /// <summary>
        /// 毫米转像素
        /// </summary>
        /// <param name="mm"></param>
        /// <returns></returns>
        public static double MmToPixel(double mm)
        {
            return PrinterUnitConvert.Convert(mm * 10, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
        }
        /// <summary>
        /// 毫米转像素
        /// </summary>
        /// <param name="mm"></param>
        /// <returns></returns>
        public static float MmToPixel(float mm)
        {
            return (float)PrinterUnitConvert.Convert(mm * 10, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
        }
        /// <summary>
        /// 毫米转像素
        /// </summary>
        /// <param name="mm"></param>
        /// <returns></returns>
        public static int MmToPixel(int mm)
        {
            return PrinterUnitConvert.Convert(mm * 10, PrinterUnit.TenthsOfAMillimeter, PrinterUnit.Display);
        }







        public static PointF getFontPx(float fontsize)
        {
            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                return new PointF(fontsize / 72 * graphics.DpiX, fontsize / 72 * graphics.DpiY);
            }
        }
        
 


   

    
    }
}
