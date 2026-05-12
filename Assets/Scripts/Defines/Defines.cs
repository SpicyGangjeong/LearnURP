using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
namespace DEFINES
{
    enum SCENES
    {
        Logo = 0,
        Title,
        Setting,
        Stage,
        Ending,
    }
    enum HRESULTS
    {
        E_FAIL = int.MinValue,
        S_OK = 1,
    }
}