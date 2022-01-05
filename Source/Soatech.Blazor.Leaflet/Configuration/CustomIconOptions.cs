namespace Soatech.Blazor.Leaflet.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CustomIconOptions
    {
        /// <summary>
        /// Name of the icon you want to show on the marker.
        /// See glyphicons or font-awesome.
        /// </summary>
        public string Icon { get; set; } = "hotel";

        /// <summary>
        /// Size of the marker icon.
        /// </summary>
        public Point IconSize { get; set; } = new Point(28, 28);

        /// <summary>
        /// Anchor size of the marker.
        /// </summary>
        public Point IconAnchor { get; set; } = new Point(14, 36);

        /// <summary>
        /// Shape of the marker icon.
        /// Possible values: marker, circle, circle-dot, rectangle, rectangle-dot, doughnut.
        /// </summary>
        public string IconShape { get; set; } = "marker";

        /// <summary>
        /// Any CSS style for the marker div.
        /// </summary>
        public string IconStyle { get; set; } = "";

        /// <summary>
        /// Anchor size of font-awesome or glyphicon with resapect to marker.
        /// </summary>
        public Point InnerIconAnchor { get; set; } = new Point(-2, 5);

        /// <summary>
        /// Any CSS style for the font-awesome or glyphicon icon.
        /// </summary>
        public string InnerIconStyle { get; set; } = "";

        /// <summary>
        /// Wether a marker with an icon or with text is created.
        /// </summary>
        public bool IsAlphaNumericIcon { get; set; } = false;

        /// <summary>
        /// If IsAlphaNumericIcon is true, this property is used to add text.
        /// </summary>
        public string Text { get; set; } = "1";

        /// <summary>
        /// Border color of the marker icon.
        /// Any in CSS usable color name or color code.
        /// </summary>
        public string BorderColor { get; set; } = "#1EB300";

        /// <summary>
        /// Border width of the marker icon.
        /// Any number, will be set to px.
        /// </summary>
        public int BorderWidth { get; set; } = 2;

        /// <summary>
        /// Border style of the marker icon.
        /// Any CSS border style.
        /// </summary>
        public string BorderStyle { get; set; } = "solid";

        /// <summary>
        /// Background color of the marker icon.
        /// Any in CSS usable color name or color code.
        /// </summary>
        public string BackgroundColor { get; set; } = "white";

        /// <summary>
        /// Text color of the marker icon.
        /// Any in CSS usable color name or color code.
        /// </summary>
        public string TextColor { get; set; } = "#1EB300";

        /// <summary>
        /// Additional custom classes in the created tag.
        /// </summary>
        public string CustomClasses { get; set; } = "";

        /// <summary>
        /// Wether the font-awesome or glyphicon icon will spin or not.
        /// </summary>
        public bool Spin { get; set; } = false;

        /// <summary>
        /// According to used icon library.
        /// fa = font-awesome, glyphicon = glyphicon.
        /// </summary>
        public string Prefix { get; set; } = "fa";

        /// <summary>
        /// Creates a marker with custom HTML.
        /// </summary>
        public string Html { get; set; } = "";
    }
}
