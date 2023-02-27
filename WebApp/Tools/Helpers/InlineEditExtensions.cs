using System;
using Omu.AwesomeMvc;

namespace WebApp.Tools.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public static class InlineEditExtensions
    {
        /// <summary>
        /// Inline hidden input for Id column, will use "Id" as name if bind or valprop is not specified
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="valProp">grid model property to get value from, when not set will use Column.Bind</param>
        /// <param name="modelProp">viewmodel property to match in the edit/create action, when not set valProp will be used</param>
        /// <returns></returns>
        public static T InlineId<T>(this T builder, string valProp = null, string modelProp = null) where T : IInlColBuilder
        {
            valProp = valProp ?? builder.Column.Bind ?? "Id";
            modelProp = modelProp ?? valProp;
            var format = "<input id='#Prefix" + modelProp + "' type='hidden' name='" + modelProp + "' value='#Value'>#Value";
            setFormat(builder, format, valProp, modelProp);
            return builder;
        }

        /// <summary>
        /// Inline hidden input
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="valProp">grid model property to get value from, when not set will use Column.Bind</param>
        /// <param name="modelProp">viewmodel property to match in the edit/create action, when not set valProp will be used</param>        
        public static T InlineHidden<T>(this T builder, string valProp = null, string modelProp = null) where T : IInlColBuilder
        {
            valProp = valProp ?? builder.Column.Bind;
            modelProp = modelProp ?? valProp;
            var format = "<input id='#Prefix" + modelProp + "' type='hidden' name='" + modelProp + "' value='#Value'>";
            setFormat(builder, format, valProp, modelProp);
            return builder;
        }

        /// <summary>
        /// set inline format for the column; value from valProp is used to replace #Value in the format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        /// <param name="builder"></param>
        /// <param name="helper">awesome editor helper</param>
        /// <param name="valProp">grid model property to get value from, when not set will use helper's Name </param>
        /// <returns></returns>
        public static T Inline<T, TH>(this T builder, IAwesomeHelper<TH> helper, string valProp = null) where T : IInlColBuilder
        {
            helper.Svalue("#Value");
            helper.Prefix("#Prefix");

            setFormat(builder, helper, valProp);

            return builder;
        }

        /// <summary>
        /// set filter format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        /// <param name="builder"></param>
        /// <param name="helper"></param>
        /// <param name="valProp">filter model property name, by default awe helper name is used</param>
        /// <returns></returns>
        public static T Filter<T, TH>(this T builder, IAwesomeHelper<TH> helper, string valProp = null) where T : IInlColBuilder
        {
            helper.Svalue("#Value");
            helper.Prefix("#Prefix");

            var formats = GetFormats(helper);
            
            builder.AddFilter(new FilterElm { format = formats.Item1, jsFormat = formats.Item2, valProp = valProp ?? helper.Awe.Prop});

            return builder;
        }

        /// <summary>
        /// Set filter format for the column
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="format">html format</param>
        /// <param name="jsFormat">js format</param>
        /// <param name="clearBtn"></param>
        /// <param name="valProp">filter model property name, by default Column.Bind is used</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T Filter<T>(this T builder,
            string format = null,
            string jsFormat = null,
            bool? clearBtn = null,
            string valProp = null,
            FilterType type = FilterType.Default) 
            where T : IInlColBuilder
        {
            string stype = null;
            if (type == FilterType.Multiselect)
            {
                stype = "multiselect";
            }
            else if (type == FilterType.Multichk)
            {
                stype = "multichk";
            }

            builder.AddFilter(new FilterElm
            {
                valProp = valProp,
                type = stype,
                format = format,
                jsFormat = jsFormat,
                clearBtn = clearBtn
            });

            return builder;
        }

        /// <summary>
        /// set inline format for the column; value from valProp is used to replace #Value in the format
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="format">editor string format, #Value and #Prefix will be replaced, prefix is used for unique ids </param>
        /// <param name="valProp">grid model property to get value from, when not set will use Column.Bind</param>
        /// <param name="modelProp">viewmodel property to match in the edit/create action, when not set valProp will be used, used for validation only here, you set the input name in the string format</param>
        /// <returns></returns>
        public static T Inline<T>(this T builder, string format, string valProp = null, string modelProp = null) where T : IInlColBuilder
        {
            setFormat(builder, format, valProp, modelProp);
            return builder;
        }

        /// <summary>
        /// set readonly inline format for the column
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="displayFormat">display format, .Prop will be replaced with values from the grid row model </param>
        /// <param name="valProp">grid model property to get value from, when not set will use Column.Bind</param>
        /// <param name="modelProp">viewmodel property to match in the edit/create action, when not set valProp will be used, used for validation only here, you set the input name in the string format</param>
        /// <returns></returns>
        public static T InlineReadonly<T>(this T builder, string displayFormat = null, string valProp = null, string modelProp = null) where T : IInlColBuilder
        {
            valProp = valProp ?? builder.Column.Bind;
            modelProp = modelProp ?? valProp;

            displayFormat = displayFormat ?? builder.Column.ClientFormat ?? "#Value";

            var format = displayFormat + "<input type='hidden' name='" + modelProp + "' value='#Value'/>";

            setFormat(builder, format, valProp, modelProp);

            return builder;
        }

        /// <summary>
        /// Inline checkbox, by default will use column.Bind + "Checked" for valueProp and column.Bind for modelProp
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="valProp">grid model property to get value from, when not set will use Column.Bind</param>
        /// <param name="modelProp">viewmodel property to match in the edit/create action, when not set valProp will be used</param>
        /// <param name="cssClass">css class for the checkbox input</param>
        /// <returns></returns>
        public static T InlineBool<T>(this T builder, string valProp = null, string modelProp = null, string cssClass = "") where T : IInlColBuilder
        {
            valProp = valProp ?? builder.Column.Bind;
            modelProp = modelProp ?? valProp;

            var format = "<input type='checkbox' name='" + modelProp + "' value='true' #ValChecked class='" + cssClass + "' />";
            setFormat(builder, format, valProp, modelProp);

            return builder;
        }

        private static void setFormat(IInlColBuilder builder, string format, string valProp, string modelProp = null, string jsformat = null)
        {
            valProp = valProp ?? builder.Column.Bind;
            modelProp = modelProp ?? valProp;

            builder.AddInline(new InlElm { format = format, modelProp = modelProp, valProp = valProp, jsFormat = jsformat });
        }

        private static void setFormat<T>(IInlColBuilder builder, IAwesomeHelper<T> helper, string valProp, string modelProp = null)
        {
            var formats = GetFormats(helper);

            setFormat(builder, formats.Item1, valProp ?? helper.Awe.Prop, modelProp ?? helper.Awe.Prop, formats.Item2);
        }

        private static Tuple<string, string> GetFormats<T>(IAwesomeHelper<T> helper)
        {
            string format, jsformat = null;
            var partRender = helper as IPartRender<object>;
            if (partRender != null)
            {
                var parts = partRender.GetParts();
                format = parts[0];
                jsformat = parts[1];
            }
            else
            {
                format = helper.ToString();
            }

            return new Tuple<string, string>(format, jsformat);
        }
    }
}