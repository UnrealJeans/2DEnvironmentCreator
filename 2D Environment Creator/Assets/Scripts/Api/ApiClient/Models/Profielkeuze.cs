using System;
using UnityEngine;

/**
 * Bijzonderheden wegens beperkingen van JsonUtility:
 * - Models hebben variabelen met kleine letters omdat JsonUtility anders de velden uit de JSON niet correct overzet naar het C# object.
 * - De id is een string in plaats van een Guid omdat JsonUtility Guid niet ondersteunt. Gelukkig geeft dit geen probleem indien we gewoon een string gebruiken in Unity en een Guid in onze backend API.
*/
[Serializable]
public class ProfielKeuze
{
    public string id; // JSON veld, string in plaats van Guid

    public string name; // JSON veld, kleine letters

    public string arts; // JSON veld, kleine letters

    public string geboorteDatum; // JSON veld, string in plaats van DateTime

    public string avatar; // JSON veld, kleine letters
}