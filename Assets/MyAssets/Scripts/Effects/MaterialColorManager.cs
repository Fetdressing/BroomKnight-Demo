using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialColorManager : MonoBehaviour {
    class StartColors
    {
        public List<Color> m_startColor = new List<Color>();
        public List<Color> m_startEmission = new List<Color>();
    }

    class WantedColorsClass //for lerping colors
    {
        public List<Color> m_wantedColors = new List<Color>();
        public List<Color> m_wantedEmissions = new List<Color>();
    }
    List<WantedColorsClass> m_wantedColors = new List<WantedColorsClass>();
    [System.NonSerialized]
    public float m_lerpingSpeedMult = 10;
    private float m_sleepAfter = 2; //how many seconds before we stop trying to lerp values after being updated
    private float m_currSleepTimer = 0.0f;
    //List<Color> m_startColor = new List<Color>();
    //List<Color> m_startEmission = new List<Color>();
    List<StartColors> m_startColors = new List<StartColors>();
    Renderer[] m_renderers;

    ColorChange m_startColorChange;
    List<ColorChange> colChangesList = new List<ColorChange>();
    // Use this for initialization
    void Awake () {

        m_currSleepTimer = 0.0f;
        m_renderers = GetComponentsInChildren<Renderer>();
        List<Renderer> meshRendOnlyTemp = new List<Renderer>();

        for(int i = 0; i < m_renderers.Length; i++) //remove any non mesh/skinnedmesh renderer from the array
        {
            if(m_renderers[i] is SkinnedMeshRenderer || m_renderers[i] is MeshRenderer)
            {
                meshRendOnlyTemp.Add(m_renderers[i]);
            }
        }
        m_renderers = meshRendOnlyTemp.ToArray(); //remove any non mesh/skinnedmesh renderer from the array

        if (m_renderers == null)
        {
            print("No renderers found");
            return;
        }

        for (int y = 0; y < m_renderers.Length; y++)
        {
            Material[] mats = m_renderers[y].materials;

            StartColors sColors = new StartColors();
            WantedColorsClass wColors = new WantedColorsClass();
            for (int i = 0; i < mats.Length; i++)
            {
                sColors.m_startColor.Add(mats[i].color);
                wColors.m_wantedColors.Add(mats[i].color);
                try //check if there is an emission colors
                {
                    sColors.m_startEmission.Add(mats[i].GetColor("_EmissionColor"));
                    wColors.m_wantedEmissions.Add(mats[i].GetColor("_EmissionColor"));
                }
                catch
                {
                    sColors.m_startEmission.Add(new Color(0, 0, 0));
                    wColors.m_wantedEmissions.Add(new Color(0, 0, 0));
                }

            }
            m_startColors.Add(sColors); //keep track of all the start colors each render has, so that we can switch back
            m_wantedColors.Add(wColors);
        }
	}

    void Update()
    {
        if(m_currSleepTimer < Time.time)
        {
            return; //we dont have to lerp if nothing has been changed for a while (performance)
        }

        for (int y = 0; y < m_wantedColors.Count; y++)
        {
            Material[] mats = m_renderers[y].materials;

            for (int i = 0; i < mats.Length; i++)
            {
                Color currColor = m_renderers[y].materials[i].color;
                Color currEmission = m_renderers[y].materials[i].GetColor("_EmissionColor"); //try if there is an emission to begin with?

                Color wanColor = m_wantedColors[y].m_wantedColors[i];
                Color wanEmission = m_wantedColors[y].m_wantedEmissions[i];

                Color newColor = Color.Lerp(currColor, wanColor, Time.deltaTime * m_lerpingSpeedMult);
                Color newEmission = Color.Lerp(currEmission, wanEmission, Time.deltaTime * m_lerpingSpeedMult);

                SetColorOnMaterialIndex(m_renderers[y], i, newColor, newEmission);

                m_renderers[y].materials = mats;
            }


            //if (colChangeToUse == null) //reset all materials to start colors
            //{
            //    for (int i = 0; i < mats.Length; i++)
            //    {
            //        SetColorOnMaterialIndex(m_renderers[y], i, m_startColors[y].m_startColor[i], m_startColors[y].m_startEmission[i]);
            //    }
            //}
            //else //equipp the new colors! :D
            //{
            //    for (int i = 0; i < mats.Length; i++)
            //    {
            //        SetColorOnMaterialIndex(m_renderers[y], i, colChangeToUse.m_color, colChangeToUse.m_emission);
            //    }
            //}

            //m_renderers[y].materials = mats;
        }
    }

    void UpdateMaterials(bool instantToggle) //doesnt need to be called all the time
    {
        ColorChange colChangeToUse = null;
        for(int i = 0; i < colChangesList.Count; i++) //find which color to use
        {
            if(colChangesList[i].m_active)
            {
                colChangeToUse = colChangesList[i];
                break;
            }
        }

        for (int y = 0; y < m_renderers.Length; y++)
        {
            Material[] mats = m_renderers[y].materials;
            if (colChangeToUse == null) //reset all materials to start colors
            {
                for (int i = 0; i < mats.Length; i++)
                {
                    //SetColorOnMaterialIndex(m_renderers[y], i, m_startColors[y].m_startColor[i], m_startColors[y].m_startEmission[i]);
                    SetWantedColorOnIndex(y, i, m_startColors[y].m_startColor[i], m_startColors[y].m_startEmission[i]);
                    if(instantToggle) //set the color instantly
                    {
                        SetColorOnMaterialIndex(m_renderers[y], i, m_startColors[y].m_startColor[i], m_startColors[y].m_startEmission[i]);
                    }
                }
            }
            else //equipp the new colors! :D
            {
                for (int i = 0; i < mats.Length; i++)
                {
                    //SetColorOnMaterialIndex(m_renderers[y], i, colChangeToUse.m_color, colChangeToUse.m_emission);
                    SetWantedColorOnIndex(y, i, colChangeToUse.m_color, colChangeToUse.m_emission);
                    if (instantToggle) //set the color instantly
                    {
                        SetColorOnMaterialIndex(m_renderers[y], i, colChangeToUse.m_color, colChangeToUse.m_emission);
                    }
                }
            }

            //m_renderers[y].materials = mats;
        }

        m_currSleepTimer = Time.time + m_sleepAfter;
    }

    protected void SetWantedColorOnIndex(int rendererIndex, int materialIndex, Color colorDiff, Color colorEmi) //apply wanted colors, these indecies are expecting the same order as the renderers and their materials
    {
        if (!(materialIndex < m_renderers[rendererIndex].materials.Length))
        {
            print("Invalid index " + materialIndex + " -Something went wrong somewhere");
            return;
        }

        m_wantedColors[rendererIndex].m_wantedColors[materialIndex] = colorDiff;
        m_wantedColors[rendererIndex].m_wantedEmissions[materialIndex] = colorEmi;
    }

    protected void SetColorOnMaterialIndex(Renderer renderer, int index, Color colorDiff, Color colorEmi) //apply color to renderer
    {
        if(!(index < renderer.materials.Length))
        {
            print("Invalid index " + index + " -Something went wrong somewhere");
            return;
        }

        renderer.materials[index].color = colorDiff;
        renderer.materials[index].SetColor("_EmissionColor", colorEmi);
    }

    public void M_AddColor(string name, int importance, Color colorDiff, Color colorEmi)
    {
        if(M_Exists(colChangesList, name))
        {
            return;
        }

        ColorChange cCh = new ColorChange(name, importance, colorDiff, colorEmi);
        colChangesList.Add(cCh);

        M_SortList();
    }

    public void M_AddColor(ColorChange colorChange)
    {
        if (M_Exists(colChangesList, colorChange.m_name))
        {
            return;
        }

        colChangesList.Add(colorChange);

        M_SortList();
    }

    public void M_ToggleColor(string name, bool active, bool instantToggle = false)
    {
        ColorChange cChange = M_GetColorChange(name);
        if(cChange != null)
        {
            cChange.m_active = active;
        }

        UpdateMaterials(instantToggle);
    }

    protected void M_SortList() //sort colChangesList so that the one with highest importance has the lowest index
    {
        List<ColorChange> newlist = new List<ColorChange>();
        int length = colChangesList.Count;
        int index = 0;
        int biggestval;

        while (index < length)
        {
            //int unsortedBiggestValue = -1000000000;
            biggestval = -100000000; int biggestIndex = 0;
            for (int i = 0; i < colChangesList.Count; i++) //find the biggest unsorted color
            {
                if(colChangesList[i].m_importance > biggestval)
                {
                    if(!M_Exists(newlist, colChangesList[i].m_name))
                    {
                        biggestIndex = i;
                        biggestval = colChangesList[i].m_importance;
                    }
                }
            }

            //add the biggest to the new list
            newlist.Add(colChangesList[biggestIndex]);
            index++;
        }

        colChangesList = newlist;
    }

    protected bool M_Exists(List<ColorChange> cChanges, string n) //returns if the colorchange already exists or not in cChanges
    {
        for(int i = 0; i < cChanges.Count; i++)
        {
            if(string.Equals(cChanges[i].m_name, n))
            {
                return true;
            }
        }
        return false;
    }

    protected ColorChange M_GetColorChange(string name)
    {
        for (int i = 0; i < colChangesList.Count; i++)
        {
            if (string.Equals(colChangesList[i].m_name, name))
            {
                return colChangesList[i];
            }
        }
        print("Couldn't find color, it doesn't exist");
        return null;
    }
}

[System.Serializable]
public class ColorChange
{
    public string m_name;
    public int m_importance; //the highest get shown if multiple are active at the same time
    [ColorUsage(true, true)]
    public Color m_color;
    [ColorUsage(true, true)]
    public Color m_emission;
    [System.NonSerialized]
    public bool m_active = false;

    public ColorChange(string name, int importance, Color colorDiff, Color colorEmi)
    {
        m_name = name;
        m_importance = importance;
        m_color = colorDiff;
        m_emission = colorEmi;
        m_active = false;
    }
}
