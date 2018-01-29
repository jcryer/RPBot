using System;
using System.Collections.Generic;
using System.Text;

namespace RPBot
{ 
    class WikiObject
    {

        public class InfoBox
        {
            public List<InfoBoxField> Fields = new List<InfoBoxField>();

            public InfoBox()
            {
                string[] infoBoxFields =
                    {
                "pagetitle", "civilian_name", "relatives", "affiliation",
                "marital_status", "age", "date_of_birth", "place_of_birth", "species", "gender",
                "height", "weight", "hair_color", "eye_color"
                };
                string[] infoBoxDescriptions =
                {
                "Please enter the character's name.", "Please enter the civilian name.", "Please enter relatives, or say `None`", "Please enter an affiliation - Hero, Villain, Rogue or Academy Student.",
                "Please enter the marital status.", "Please enter the age.", "Pleae enter the date of birth.", "Please enter the place of birth.", "Please enter the species.", "Please enter the gender.",
                "Please enter the height.", "Please enter the weight.", "Please enter the hair colour.", "Please enter the eye colour."
                };

                for (int i = 0; i < infoBoxFields.Length; i++)
                {
                    Fields.Add(new InfoBoxField(infoBoxFields[i], "", infoBoxDescriptions[i]));
                }
            }

            public string BuildInfoBox(string image)
            {
                string infoBoxString = "{{Character|" + image;
                foreach (InfoBoxField field in Fields)
                {
                    if (!string.IsNullOrWhiteSpace(field.FieldValue)) {
                        if (field.FieldValue != "-") infoBoxString += field.FieldId + " = " + field.FieldValue + "|";
                    }
                }
                infoBoxString = infoBoxString.TrimEnd('|') + "}}";
                return infoBoxString;
            }
        }
        public class InfoBoxField
        {
            public string FieldId;
            public string FieldValue;
            public string Question;

            public InfoBoxField(string fieldId, string fieldValue, string question)
            {
                FieldId = fieldId;
                FieldValue = fieldValue;
                Question = question;
            }
        }
    }
}
