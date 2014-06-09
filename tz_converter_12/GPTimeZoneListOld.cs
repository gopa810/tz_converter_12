using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using GCAL.Base;

namespace tz_converter_12
{
    public class GPTimeZoneListOld
    {
        public bool Modified = false;
        private XmlDocument tzdoc = null;
        private XmlNode mainNode = null;
        private List<GPTimeZoneOld> tzonesList = null;
        private int LastId = 1;
        public List<string> log = new List<string>();

        /// <summary>
        /// do not merge
        /// </summary>
        /// <returns></returns>
        public XmlDocument getTimezonesXml()
        {
            if (tzdoc == null)
            {
                Assembly assem = this.GetType().Assembly;

                XmlDocument doc = new XmlDocument();

                //doc.Load("d:\\gcal\\GCAL\\GCAL.Base\\Files\\Timezones.xml");
                doc.Load("c:\\Users\\peter.kollath\\Documents\\GitHub\\GCAL\\GCAL.Base\\Files\\Timezones.xml");

                tzdoc = doc;
            }
            return tzdoc;
        }


        public int dayOfWeek(DayOfWeek dow)
        {
            if (dow == DayOfWeek.Friday) return 5;
            if (dow == DayOfWeek.Monday) return 1;
            if (dow == DayOfWeek.Saturday) return 6;
            if (dow == DayOfWeek.Sunday) return 0;
            if (dow == DayOfWeek.Thursday) return 4;
            if (dow == DayOfWeek.Tuesday) return 2;
            return 3;
        }

        /// <summary>
        /// do not merge
        /// </summary>
        /// 
        public GPTimeZoneList convertTimezones()
        {
            GPTimeZoneList list = new GPTimeZoneList();
            int c1;
            int wom = 0;
            GPTimeZoneOld.Rule activeRule = null;

            foreach (GPTimeZoneOld tz in tzonesList)
            {
                if (tz.Transitions.Count > 0)
                {
                    for (int i = 0; i < tz.Transitions.Count; i++)
                    {
                        try
                        {
                            GPTimeZoneOld.Transition trans = tz.Transitions[i];
                            DateTime dt = trans.getDateTime(tz.OffsetSeconds);
                            c1 = tz.getTransCountForYear(dt.Year);
                            if (c1 == 2)
                            {
                                trans.Paired = true;
                            }
                            if (c1 > 2)
                            {
                                log.Add("More than 2 transition in year " + dt.Year + " for timezone " + tz.Name + "\n");
                            }

                            //trans.Rule1 = string.Format("{0}-{1}", dt.Month, dt.Day);
/*                            DateTime dta = new DateTime(dt.Year, dt.Month, 1);
                            int womFdm = (dayOfWeek(dta.DayOfWeek) + 6)%7;
                            int virt = 1 - womFdm;
                            dta = dta.AddDays((7 - womFdm)%7);
                            wom = 1;
                            for (int y = 0; y < 10; y++)
                            {
                                if (dt < dta)
                                {
                                    if (dta.Month != dt.Month)
                                    {
                                        wom = 5;
                                    }
                                    break;
                                }
                                dta = dta.AddDays(7);
                                wom++;
                            }
*/
                            // TODO: correct offset determination should be applied
                            // now there are difreneces against actual offset
                            int sd = dt.Day;
                            while (sd > 0)
                            {
                                wom++;
                                sd -= 7;
                            }
                            if (wom > 5)
                                wom = 5; 
                            trans.Rule2 = string.Format("{0}-{1}-{2}-{3:00}", dt.Month, wom, dayOfWeek(dt.DayOfWeek), dt.Hour);

                            if (trans.Dst == true)
                            {
                                activeRule = new GPTimeZoneOld.Rule();
                                activeRule.OffsetStart = trans.OffsetInSeconds / 60;
                                activeRule.RuleTextStart = trans.Rule2;
                                activeRule.YearStart = dt.Year;
                            }
                            else if (trans.Dst == false)
                            {
                                if (activeRule != null)
                                {
                                    activeRule.OffsetEnd = trans.OffsetInSeconds / 60;
                                    activeRule.RuleTextEnd = trans.Rule2;
                                    activeRule.YearEnd = dt.Year;
                                    tz.Rules.Add(activeRule);
                                    activeRule = null;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Add(tz.Name + " - exception: " + ex.Message);
                        }
                    }
                }

                c1 = tz.getTransCountForYear(2014) + tz.getTransCountForYear(2015);
                tz.UsingDst = (c1 > 0);

                if (tz.UsingDst)
                {
                    tz.SymbolNormalTime = tz.getTimeAbbr(false);
                    tz.SymbolDaylightTime = tz.getTimeAbbr(true);

                    int idx = tz.Transitions.Count - 1;
                    GPTimeZoneOld.Transition tr = tz.Transitions[idx];
                    if (tr.Paired == false && tr.getDateTime().Year > 2040)
                        tz.Transitions.RemoveAt(idx);

                    /*tr = tz.Transitions[0];
                    while (true)
                    {
                        if (tr.Paired == false && tr.getDateTime().Year < 2005)
                            tz.Transitions.RemoveAt(0);
                        else
                            break;
                        if (tz.Transitions.Count > 0)
                            tr = tz.Transitions[0];
                    }*/
                }
                else
                {
                    GPTimeZoneOld.Transition tr = tz.Transitions[tz.Transitions.Count - 1];
                    tz.OffsetSeconds = tr.OffsetInSeconds;
                    tz.SymbolNormalTime = tr.Abbreviation;
                    tz.SymbolDaylightTime = tr.Abbreviation;
                    tz.Transitions.Clear();
                    tz.Rules.Clear();
                }

                for (int u = 0; u < tz.Rules.Count; u++)
                {
                    if (u + 1 < tz.Rules.Count)
                    {
                        if (tz.Rules[u].OffsetStart == tz.Rules[u + 1].OffsetStart
                            && tz.Rules[u].OffsetEnd == tz.Rules[u + 1].OffsetEnd
                            && tz.Rules[u].RuleTextEnd == tz.Rules[u + 1].RuleTextEnd
                            && tz.Rules[u].RuleTextStart == tz.Rules[u + 1].RuleTextStart
                            )
                        {
                            tz.Rules[u].YearEnd = tz.Rules[u + 1].YearEnd;
                            tz.Rules.RemoveAt(u + 1);
                            u--;
                        }
                    }
                }
                for (int u = 0; u < tz.Rules.Count; u++)
                {
                    if (tz.Rules[u].YearEnd > 2050)
                        tz.Rules[u].YearEnd = 2200;
                    if ((tz.Rules[u].YearEnd - tz.Rules[u].YearStart < 4)
                        )
                    {
                        tz.Rules.RemoveAt(u);
                        u--;
                    }
                }
                foreach (GPTimeZoneOld.Rule rule in tz.Rules)
                {
                    tz.RemoveTransitionsForRule(rule);
                }
                for (int u = tz.Transitions.Count - 1; u >= 0; u--)
                {
                    if (tz.Transitions[u].OffsetInSeconds == tz.OffsetSeconds)
                    {
                        tz.Transitions.RemoveAt(u);
                    }
                }
            }


            return list;
        }





        private static GPTimeZoneListOld tzlist = null;

        public static GPTimeZoneListOld sharedTimeZones()
        {
            if (tzlist == null)
            {
                tzlist = new GPTimeZoneListOld();
            }
            return tzlist;
        }

        public XmlNode getMainNode()
        {
            if (mainNode == null)
            {
                XmlDocument doc = getTimezonesXml();
                XmlNodeList elems = doc.ChildNodes;
                foreach (XmlNode xnode in elems)
                {
                    if (xnode.Name == "timezone_list")
                    {
                        mainNode = xnode;
                        break;
                    }
                }
            }
            return mainNode;
        }

        public GPTimeZoneOld GetTimezoneByName(string name)
        {
            foreach (GPTimeZoneOld tz in getTimeZones())
            {
                if (tz.Name == name)
                    return tz;
            }
            return null;
        }

        public List<GPTimeZoneOld> getTimeZones()
        {
            if (tzonesList == null)
            {
                tzonesList = new List<GPTimeZoneOld>();
                XmlNode node = getMainNode();
                XmlElement elem = node as XmlElement;
                foreach (XmlElement item in elem.ChildNodes)
                {
                    if (item.Name == "timezone")
                    {
                        GPTimeZoneOld tzone = new GPTimeZoneOld();
                        tzone.Id = LastId++;
                        foreach (XmlElement subs in item.ChildNodes)
                        {
                            if (subs.Name == "name")
                            {
                                tzone.Name = subs.InnerText;
                            }
                            else if (subs.Name == "transition")
                            {
                                GPTimeZoneOld.Transition trans = new GPTimeZoneOld.Transition();
                                if (subs.HasAttribute("date"))
                                {
                                    trans.setDateString(subs.GetAttribute("date"));
                                }
                                if (subs.HasAttribute("offset"))
                                {
                                    int.TryParse(subs.GetAttribute("offset"), out trans.OffsetInSeconds);
                                }
                                if (subs.HasAttribute("abbr"))
                                {
                                    trans.Abbreviation = subs.GetAttribute("abbr");
                                }
                                if (subs.HasAttribute("dst"))
                                {
                                    int dst = 0;
                                    int.TryParse(subs.GetAttribute("dst"), out dst);
                                    trans.Dst = ((dst == 1) ? true : false);
                                    if (trans.Dst == false)
                                    {
                                        tzone.OffsetSeconds = trans.OffsetInSeconds;
                                    }
                                }
                                tzone.Transitions.Add(trans);
                            }
                        }
                        tzone.RefreshEnds();
                        if (tzone.Name.Length > 0)
                            tzonesList.Add(tzone);

                    }
                }

            }


            return tzonesList;
        }


        /// <summary>
        /// Create XML document as representation of complete timezone data
        /// </summary>
        /// <returns>XMLDocument</returns>
        public XmlDocument generateXmlDocument()
        {
            XmlDocument doc = new XmlDocument();

            XmlElement root = doc.CreateElement("timezone_list");
            doc.AppendChild(root);

            XmlElement tzoneNode = null;
            XmlElement tzoneTransNode = null;
            XmlElement child = null;

            foreach (GPTimeZoneOld tzone in getTimeZones())
            {
                tzoneNode = doc.CreateElement("timezone");
                root.AppendChild(tzoneNode);

                tzoneNode.SetAttribute("normalAbbr", tzone.SymbolNormalTime);
                tzoneNode.SetAttribute("dstAbbr", tzone.SymbolDaylightTime);
                tzoneNode.SetAttribute("offset", (tzone.OffsetSeconds / 60).ToString());
                tzoneNode.SetAttribute("dst", tzone.UsingDst.ToString());

                child = doc.CreateElement("name");
                tzoneNode.AppendChild(child);
                child.InnerText = tzone.Name;

                foreach (GPTimeZoneOld.Transition trans in tzone.Transitions)
                {
                    tzoneTransNode = doc.CreateElement("transition");
                    tzoneNode.AppendChild(tzoneTransNode);

                    //tzoneTransNode.SetAttribute("pair", trans.Paired ? "1" : "0");
                    //tzoneTransNode.SetAttribute("rule", trans.Rule2);
                    tzoneTransNode.SetAttribute("date", trans.getDateStringx(trans.Timestamp, tzone.OffsetSeconds));
                    tzoneTransNode.SetAttribute("datend", trans.getDateStringx(trans.TimestampEnd, tzone.OffsetSeconds));
                    tzoneTransNode.SetAttribute("offset", (trans.OffsetInSeconds / 60).ToString());
                    //tzoneTransNode.SetAttribute("abbr", trans.Abbreviation);
                    //tzoneTransNode.SetAttribute("dst", (trans.Dst ? "1" : "0"));
                }

                foreach (GPTimeZoneOld.Rule rule in tzone.Rules)
                {
                    child = doc.CreateElement("rule");
                    tzoneNode.AppendChild(child);

                    child.SetAttribute("ruleStart", rule.RuleTextStart);
                    child.SetAttribute("ruleEnd", rule.RuleTextEnd);
                    child.SetAttribute("yearStart", rule.YearStart.ToString());
                    child.SetAttribute("yearEnd", rule.YearEnd.ToString());
                    child.SetAttribute("offset", rule.OffsetStart.ToString());
                    //child.SetAttribute("offsetEnd", rule.OffsetEnd.ToString());
                }
            }

            return doc;
        }

        public void saveXml(string fileName)
        {
            XmlDocument doc = generateXmlDocument();

            doc.Save(fileName);
        }

        ~GPTimeZoneListOld()
        {
            if (Modified)
            {
                string fileName = "Timezones.xml";
                string dir = GPFileHelper.getAppDataDirectory();
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                fileName = Path.Combine(dir, fileName);
                saveXml(fileName);
            }
        }

        private class rec 
        {
            public int sec;
            public string text;
        }

        public string getTimezonesOffsetListDesc()
        {
            GPSortedIntStringList sl = new GPSortedIntStringList();
            foreach (GPTimeZoneOld tz in getTimeZones())
            {
                sl.push(Convert.ToInt32(tz.OffsetSeconds), "UTC " + tz.getOffsetString());
            }

            return sl.ToString();
        }

        public GPTimeZoneOld GetTimezoneById(int id)
        {
            foreach (GPTimeZoneOld tz in getTimeZones())
            {
                if (tz.Id == id)
                    return tz;
            }
            return null;
        }
    }
}
