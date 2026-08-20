using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Progresso
{
    public static bool serpenteDerrotada = false;
    public static bool mulherDerrotada = false;
    public static bool tlalocDerrotado = false;

    public static void ResetarProgresso()
    {
        serpenteDerrotada = false;
        mulherDerrotada = false;
        tlalocDerrotado = false;
    }

    public static bool TodasFasesCompletas()
    {
        return serpenteDerrotada && mulherDerrotada && tlalocDerrotado;
    }
}
