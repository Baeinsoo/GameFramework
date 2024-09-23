using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class WebRequestException : Exception
    {
        public WebRequestException(string message) : base(message) { }
    }
}
