﻿namespace TheWorld.Services
{
    public class GeoCoordsResult
    {
        public bool Success { get; set; }
        public string  Message { get; set; }
        public double Longtude { get; set; }
        public double Latitude { get; set; }
    }
}