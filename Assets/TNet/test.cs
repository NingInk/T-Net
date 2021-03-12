using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public int index = 100;
    public GameObject gg;
    GameObject[,] ll;
    void Start()
    {
        ll = new GameObject[index, index];
        for (int i = 0; i < index; i++)
        {
            for (int j = 0; j < index; j++)
            {
                GameObject go = Instantiate(gg, new Vector3(i, 0, j), Quaternion.identity);
                Mycube mc = go.AddComponent<Mycube>();
                mc.x = i;
                mc.y = j;
                mc.index = index;
                ll[i, j] = go;
            }
        }
        gos = new List<GameObject>();
        a = gos.Count;
    }



    int a;
    public static List<GameObject> gos;

    public List<GameObject> oldgo;
    public List<GameObject> newgo;

    void Update()
    {
        if (gos.Count != a)
        {
            a = gos.Count;
            oldgo = new List<GameObject>();
            newgo = new List<GameObject>();
            dododo();
        }
    }


    void dododo()
    {
        foreach (var item in gos)
        {
            float i = Random.Range(0f, index);
            item.transform.localScale = new Vector3(1, i, 1);
            oldgo.Add(item);
            newgo.AddRange(getnew(item, i));
        }
        StartCoroutine(ceshi());
    }

    List<GameObject> getnew(GameObject go, float a)
    {
        List<GameObject> lg = new List<GameObject>();
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (!oldgo.Contains(ll[go.GetComponent<Mycube>().x + i, go.GetComponent<Mycube>().y + j]))
                {
                    GameObject goj = ll[go.GetComponent<Mycube>().x + i, go.GetComponent<Mycube>().y + j];
                    goj.GetComponent<Mycube>().h = a;
                    lg.Add(goj);
                }
            }
        }
        return lg;
    }
    IEnumerator ceshi()
    {
        List<GameObject> gogo = new List<GameObject>();
        List<GameObject> gog = new List<GameObject>();

        foreach (var item in newgo)
        {
            float i = item.GetComponent<Mycube>().h - 1;
            item.transform.localScale = new Vector3(1, i, 1);
            gogo.Add(item);
            oldgo.Add(item);
            gog.AddRange(getnew(item, i));
            yield return null;
        }
        newgo.AddRange(gog);

        for (int i = 0; i < gogo.Count; i++)
        {
            if (newgo.Contains(gogo[i]))
                newgo.Remove(gogo[i]);
        }
        StartCoroutine(ceshi());
        yield break;
    }
}


public class Mycube : MonoBehaviour
{
    public int x, y;
    public float h;
    public float index = 100;
    private void OnMouseDown()
    {
        test.gos.Add(gameObject);
    }
    private void Update()
    {
        float f = 100f - transform.localScale.y;

        float i = f * (2f / 100f);

        float l = i - 1f;
        Color col = new Color(1, Mathf.Clamp(i, 0f, 1f), Mathf.Clamp(l, 0f, 1f));

        GetComponent<Renderer>().material.color = col;
    }
}