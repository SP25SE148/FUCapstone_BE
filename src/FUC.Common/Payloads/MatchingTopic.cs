﻿using System.Text.Json.Serialization;

namespace FUC.Common.Payloads;

public class MatchingTopic
{
    [JsonPropertyName("similarity")] public double Similarity { get; set; }

    [JsonPropertyName("english_name")] public string EnglishName { get; set; }
}
