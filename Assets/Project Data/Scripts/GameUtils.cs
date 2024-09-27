using System;
using UnityEngine;

namespace Project_Data.Scripts
{
    public class GameUtils
    {
        public static string currencyToString(double valueToConvert, int round = 2)
        {
            valueToConvert = Math.Round(valueToConvert, round);
            string converted;
            if (Math.Log10(valueToConvert) >= 306) { converted = ((valueToConvert / 1e306)).ToString("N") + " dt"; }
            else if (Math.Log10(valueToConvert) >= 303) { converted = ((valueToConvert / 1e303)).ToString("N") + " ds"; }
            else if (Math.Log10(valueToConvert) >= 300) { converted = ((valueToConvert / 1e300)).ToString("N") + " dr"; }
            else if (Math.Log10(valueToConvert) >= 297) { converted = ((valueToConvert / 1e297)).ToString("N") + " dq"; }
            else if (Math.Log10(valueToConvert) >= 294) { converted = ((valueToConvert / 1e294)).ToString("N") + " dp"; }
            else if (Math.Log10(valueToConvert) >= 291) { converted = ((valueToConvert / 1e291)).ToString("N") + " do"; }
            else if (Math.Log10(valueToConvert) >= 288) { converted = ((valueToConvert / 1e288)).ToString("N") + " dn"; }
            else if (Math.Log10(valueToConvert) >= 285) { converted = ((valueToConvert / 1e285)).ToString("N") + " dm"; }
            else if (Math.Log10(valueToConvert) >= 282) { converted = ((valueToConvert / 1e282)).ToString("N") + " dl"; }
            else if (Math.Log10(valueToConvert) >= 279) { converted = ((valueToConvert / 1e279)).ToString("N") + " dk"; }
            else if (Math.Log10(valueToConvert) >= 276) { converted = ((valueToConvert / 1e276)).ToString("N") + " dj"; }
            else if (Math.Log10(valueToConvert) >= 273) { converted = ((valueToConvert / 1e273)).ToString("N") + " di"; }
            else if (Math.Log10(valueToConvert) >= 270) { converted = ((valueToConvert / 1e270)).ToString("N") + " dh"; }
            else if (Math.Log10(valueToConvert) >= 267) { converted = ((valueToConvert / 1e267)).ToString("N") + " dg"; }
            else if (Math.Log10(valueToConvert) >= 264) { converted = ((valueToConvert / 1e264)).ToString("N") + " df"; }
            else if (Math.Log10(valueToConvert) >= 261) { converted = ((valueToConvert / 1e261)).ToString("N") + " de"; }
            else if (Math.Log10(valueToConvert) >= 258) { converted = ((valueToConvert / 1e258)).ToString("N") + " dd"; }
            else if (Math.Log10(valueToConvert) >= 255) { converted = ((valueToConvert / 1e255)).ToString("N") + " dc"; }
            else if (Math.Log10(valueToConvert) >= 252) { converted = ((valueToConvert / 1e252)).ToString("N") + " db"; }
            else if (Math.Log10(valueToConvert) >= 249) { converted = ((valueToConvert / 1e249)).ToString("N") + " da"; }
            else if (Math.Log10(valueToConvert) >= 246) { converted = ((valueToConvert / 1e246)).ToString("N") + " cz"; }
            else if (Math.Log10(valueToConvert) >= 243) { converted = ((valueToConvert / 1e243)).ToString("N") + " cy"; }
            else if (Math.Log10(valueToConvert) >= 240) { converted = ((valueToConvert / 1e240)).ToString("N") + " cx"; }
            else if (Math.Log10(valueToConvert) >= 237) { converted = ((valueToConvert / 1e237)).ToString("N") + " cw"; }
            else if (Math.Log10(valueToConvert) >= 234) { converted = ((valueToConvert / 1e234)).ToString("N") + " cv"; }
            else if (Math.Log10(valueToConvert) >= 231) { converted = ((valueToConvert / 1e231)).ToString("N") + " cu"; }
            else if (Math.Log10(valueToConvert) >= 228) { converted = ((valueToConvert / 1e228)).ToString("N") + " ct"; }
            else if (Math.Log10(valueToConvert) >= 225) { converted = ((valueToConvert / 1e225)).ToString("N") + " cs"; }
            else if (Math.Log10(valueToConvert) >= 222) { converted = ((valueToConvert / 1e222)).ToString("N") + " cr"; }
            else if (Math.Log10(valueToConvert) >= 219) { converted = ((valueToConvert / 1e219)).ToString("N") + " cq"; }
            else if (Math.Log10(valueToConvert) >= 216) { converted = ((valueToConvert / 1e216)).ToString("N") + " cp"; }
            else if (Math.Log10(valueToConvert) >= 213) { converted = ((valueToConvert / 1e213)).ToString("N") + " co"; }
            else if (Math.Log10(valueToConvert) >= 210) { converted = ((valueToConvert / 1e210)).ToString("N") + " cn"; }
            else if (Math.Log10(valueToConvert) >= 207) { converted = ((valueToConvert / 1e207)).ToString("N") + " cm"; }
            else if (Math.Log10(valueToConvert) >= 204) { converted = ((valueToConvert / 1e204)).ToString("N") + " cl"; }
            else if (Math.Log10(valueToConvert) >= 201) { converted = ((valueToConvert / 1e201)).ToString("N") + " ck"; }
            else if (Math.Log10(valueToConvert) >= 198) { converted = ((valueToConvert / 1e198)).ToString("N") + " cj"; }
            else if (Math.Log10(valueToConvert) >= 195) { converted = ((valueToConvert / 1e195)).ToString("N") + " ci"; }
            else if (Math.Log10(valueToConvert) >= 192) { converted = ((valueToConvert / 1e192)).ToString("N") + " ch"; }
            else if (Math.Log10(valueToConvert) >= 189) { converted = ((valueToConvert / 1e198)).ToString("N") + " cg"; }
            else if (Math.Log10(valueToConvert) >= 186) { converted = ((valueToConvert / 1e186)).ToString("N") + " cf"; }
            else if (Math.Log10(valueToConvert) >= 183) { converted = ((valueToConvert / 1e183)).ToString("N") + " ce"; }
            else if (Math.Log10(valueToConvert) >= 180) { converted = ((valueToConvert / 1e180)).ToString("N") + " cd"; }
            else if (Math.Log10(valueToConvert) >= 177) { converted = ((valueToConvert / 1e177)).ToString("N") + " cc"; }
            else if (Math.Log10(valueToConvert) >= 174) { converted = ((valueToConvert / 1e174)).ToString("N") + " cb"; }
            else if (Math.Log10(valueToConvert) >= 171) { converted = ((valueToConvert / 1e171)).ToString("N") + " ca"; }
            else if (Math.Log10(valueToConvert) >= 168) { converted = ((valueToConvert / 1e168)).ToString("N") + " bz"; }
            else if (Math.Log10(valueToConvert) >= 165) { converted = ((valueToConvert / 1e165)).ToString("N") + " by"; }
            else if (Math.Log10(valueToConvert) >= 162) { converted = ((valueToConvert / 1e162)).ToString("N") + " bx"; }
            else if (Math.Log10(valueToConvert) >= 159) { converted = ((valueToConvert / 1e159)).ToString("N") + " bw"; }
            else if (Math.Log10(valueToConvert) >= 156) { converted = ((valueToConvert / 1e156)).ToString("N") + " bv"; }
            else if (Math.Log10(valueToConvert) >= 153) { converted = ((valueToConvert / 1e153)).ToString("N") + " bu"; }
            else if (Math.Log10(valueToConvert) >= 150) { converted = ((valueToConvert / 1e150)).ToString("N") + " bt"; }
            else if (Math.Log10(valueToConvert) >= 147) { converted = ((valueToConvert / 1e147)).ToString("N") + " bs"; }
            else if (Math.Log10(valueToConvert) >= 144) { converted = ((valueToConvert / 1e144)).ToString("N") + " br"; }
            else if (Math.Log10(valueToConvert) >= 141) { converted = ((valueToConvert / 1e141)).ToString("N") + " bq"; }
            else if (Math.Log10(valueToConvert) >= 138) { converted = ((valueToConvert / 1e138)).ToString("N") + " bp"; }
            else if (Math.Log10(valueToConvert) >= 135) { converted = ((valueToConvert / 1e135)).ToString("N") + " bo"; }
            else if (Math.Log10(valueToConvert) >= 132) { converted = ((valueToConvert / 1e132)).ToString("N") + " bn"; }
            else if (Math.Log10(valueToConvert) >= 129) { converted = ((valueToConvert / 1e129)).ToString("N") + " bm"; }
            else if (Math.Log10(valueToConvert) >= 126) { converted = ((valueToConvert / 1e126)).ToString("N") + " bl"; }
            else if (Math.Log10(valueToConvert) >= 123) { converted = ((valueToConvert / 1e123)).ToString("N") + " bk"; }
            else if (Math.Log10(valueToConvert) >= 120) { converted = ((valueToConvert / 1e120)).ToString("N") + " bj"; }
            else if (Math.Log10(valueToConvert) >= 117) { converted = ((valueToConvert / 1e117)).ToString("N") + " bi"; }
            else if (Math.Log10(valueToConvert) >= 114) { converted = ((valueToConvert / 1e114)).ToString("N") + " bh"; }
            else if (Math.Log10(valueToConvert) >= 111) { converted = ((valueToConvert / 1e111)).ToString("N") + " bg"; }
            else if (Math.Log10(valueToConvert) >= 108) { converted = ((valueToConvert / 1e108)).ToString("N") + " bf"; }
            else if (Math.Log10(valueToConvert) >= 105) { converted = ((valueToConvert / 1e105)).ToString("N") + " be"; }
            else if (Math.Log10(valueToConvert) >= 102) { converted = ((valueToConvert / 1e102)).ToString("N") + " bd"; }
            else if (Math.Log10(valueToConvert) >= 99) { converted = ((valueToConvert / 1e99)).ToString("N") + " bc"; }
            else if (Math.Log10(valueToConvert) >= 96) { converted = ((valueToConvert / 1e96)).ToString("N") + " bb"; }
            else if (Math.Log10(valueToConvert) >= 93) { converted = ((valueToConvert / 1e93)).ToString("N") + " ba"; }
            else if (Math.Log10(valueToConvert) >= 90) { converted = ((valueToConvert / 1e90)).ToString("N") + " az"; }
            else if (Math.Log10(valueToConvert) >= 87) { converted = ((valueToConvert / 1e87)).ToString("N") + " ay"; }
            else if (Math.Log10(valueToConvert) >= 84) { converted = ((valueToConvert / 1e84)).ToString("N") + " ax"; }
            else if (Math.Log10(valueToConvert) >= 81) { converted = ((valueToConvert / 1e81)).ToString("N") + " aw"; }
            else if (Math.Log10(valueToConvert) >= 78) { converted = ((valueToConvert / 1e78)).ToString("N") + " av"; }
            else if (Math.Log10(valueToConvert) >= 75) { converted = ((valueToConvert / 1e75)).ToString("N") + " au"; }
            else if (Math.Log10(valueToConvert) >= 72) { converted = ((valueToConvert / 1e72)).ToString("N") + " at"; }
            else if (Math.Log10(valueToConvert) >= 69) { converted = ((valueToConvert / 1e69)).ToString("N") + " as"; }
            else if (Math.Log10(valueToConvert) >= 66) { converted = ((valueToConvert / 1e66)).ToString("N") + " ar"; }
            else if (Math.Log10(valueToConvert) >= 63) { converted = ((valueToConvert / 1e63)).ToString("N") + " aq"; }
            else if (Math.Log10(valueToConvert) >= 60) { converted = ((valueToConvert / 1e60)).ToString("N") + " ap"; }
            else if (Math.Log10(valueToConvert) >= 57) { converted = ((valueToConvert / 1e57)).ToString("N") + " ao"; }
            else if (Math.Log10(valueToConvert) >= 54) { converted = ((valueToConvert / 1e54)).ToString("N") + " an"; }
            else if (Math.Log10(valueToConvert) >= 51) { converted = ((valueToConvert / 1e51)).ToString("N") + " am"; }
            else if (Math.Log10(valueToConvert) >= 48) { converted = ((valueToConvert / 1e48)).ToString("N") + " al"; }
            else if (Math.Log10(valueToConvert) >= 45) { converted = ((valueToConvert / 1e45)).ToString("N") + " ak"; }
            else if (Math.Log10(valueToConvert) >= 42) { converted = ((valueToConvert / 1e42)).ToString("N") + " aj"; }
            else if (Math.Log10(valueToConvert) >= 39) { converted = ((valueToConvert / 1e39)).ToString("N") + " ai"; }
            else if (Math.Log10(valueToConvert) >= 36) { converted = ((valueToConvert / 1e36)).ToString("N") + " ah"; }
            else if (Math.Log10(valueToConvert) >= 33) { converted = ((valueToConvert / 1e33)).ToString("N") + " ag"; }
            else if (Math.Log10(valueToConvert) >= 30) { converted = ((valueToConvert / 1e30)).ToString("N") + " af"; }
            else if (Math.Log10(valueToConvert) >= 27) { converted = ((valueToConvert / 1e27)).ToString("N") + " ae"; }
            else if (Math.Log10(valueToConvert) >= 24) { converted = ((valueToConvert / 1e24)).ToString("N") + " ad"; }
            else if (Math.Log10(valueToConvert) >= 21) { converted = ((valueToConvert / 1e21)).ToString("N") + " ac"; }
            else if (Math.Log10(valueToConvert) >= 18) { converted = ((valueToConvert / 1e18)).ToString("N") + " ab"; }
            else if (Math.Log10(valueToConvert) >= 15) { converted = ((valueToConvert / 1e15)).ToString("N") + " aa"; }
            else if (Math.Log10(valueToConvert) >= 12) { converted = ((valueToConvert / 1e12)).ToString("N") + " T"; }
            else if (Math.Log10(valueToConvert) >= 9) { converted = ((valueToConvert / 1e9)).ToString("N") + " B"; }
            else if (Math.Log10(valueToConvert) >= 6) { converted = ((valueToConvert / 1e6)).ToString("N") + " M"; }
            else if (Math.Log10(valueToConvert) >= 3) { converted = ((valueToConvert / 1000)).ToString("N") + " K"; }
            else
            {
                converted = "" + valueToConvert.ToString("N");
            }


            string [] test = converted.Split(new char[] { ' ' });

            if (test.Length == 0)
            {
                Debug.LogError("NO SPACE");
            }
            else if (test.Length == 1)
            {
                double amount = Double.Parse(test[0]);
                converted = convertToThreeDigits(amount);
            }
            else if (test.Length == 2)
            {
                double amount = Double.Parse(test[0]);
                converted = convertToThreeDigits(amount) + " " + test[1];
            }
        
            return converted;

        }

        public static string convertToThreeDigits(double amount)
        {
            if (amount < 10)
            {
                amount = Math.Round(amount, 2);
            }
            else if (amount < 100)
            {
                amount = Math.Round(amount, 1);
            }
            else if (amount < 1000)
            {
                amount = Math.Round(amount, 0);
            }
            string converted = "" + amount.ToString();
            return converted;
        }

        public static int getNextUpgradeLevel(int level)
        {
            int nextUpgradeLevel;
            for (int i = 0; ; i++)
            {
                nextUpgradeLevel = 25 * (int)Math.Pow(2, i);

                if (level < nextUpgradeLevel)
                {
                    break;
                }
            }

            return nextUpgradeLevel;
        }

        public static int getPreviousUpgradeLevel(int level)
        {
            int previousUpgradeLevel;
            for (int i = 0; ; i++)
            {
                previousUpgradeLevel = 25 * (int)Math.Pow(2, i);

                if (level < previousUpgradeLevel)
                {
                    previousUpgradeLevel = 25 * (int)Math.Pow(2, i - 1);
                    break;
                }
            }

            return previousUpgradeLevel;
        }

        public static int getUpgradeCount(int level)
        {
            int nextUpgradeLevel = 0;
            for (int i = 0; ; i++)
            {
                nextUpgradeLevel = 25 * (int)Math.Pow(2, i);

                if (level < nextUpgradeLevel)
                {
                    return i;
                }
            }
        }


        public static string TimeSpanToReadableString(TimeSpan span)
        {
            string formatted = string.Format("{0}{1}{2}{3}",
                span.Duration().Days > 0 ?  string.Format("{0:0}d ", span.Days)  : String.Empty,
                span.Duration().Hours > 0 ? string.Format("{0:0}h ", span.Hours) : String.Empty,
                span.Duration().Minutes > 0 ? string.Format("{0:0}m ", span.Minutes) : "0m, ",
                span.Duration().Seconds > 0 ? string.Format("{0:0}s", span.Seconds)  : "0s");

            if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

            if (string.IsNullOrEmpty(formatted)) formatted = "0 sec";

            return formatted;
        }

        public static string TimeSpanToReadableString1(TimeSpan span)
        {
            string formatted = string.Format("{0}{1}{2}{3}",
                span.Duration().Days > 0 ?  string.Format("{0:0}d ", span.Days)  : String.Empty,
                span.Duration().Hours > 0 ? string.Format("{0:0}h ", span.Hours) : String.Empty,
                span.Duration().Minutes > 0 ? string.Format("{0:0}m ", span.Minutes) : String.Empty,
                span.Duration().Seconds > 0 ? string.Format("{0:0}s", span.Seconds)  : "0s");

            if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

            if (string.IsNullOrEmpty(formatted)) formatted = "0 sec";

            return formatted;
        }
    }
}