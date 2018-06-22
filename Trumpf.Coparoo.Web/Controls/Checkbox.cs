﻿namespace Trumpf.Coparoo.Web.Controls
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Interactions;

    /// <summary>
    /// Checkbox control object.
    /// Expects an input html element with attribute type="checkbox".
    /// </summary>
    public class Checkbox : ControlObject
    {
        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        protected override By SearchPattern => By.XPath(".//input[@type='checkbox']");

        /// <summary>
        /// Gets or sets a value indicating whether the checkbox is checked.
        /// </summary>
        public bool Checked
        {
            get { return Node.GetAttribute("checked") != null; }

            set
            {
                if (Checked != value)
                {
                    new Actions(Root.Driver).MoveToElement(Node).Click().Perform();
                }
            }
        }
    }
}
