using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour {
    // Start is called before the first frame update
    [Header ("========= Actor Settings =========")]
    public int Health = 10;
    public int MaxHealth = 10;
    public int Damage = 2;

    public Renderer[] Meshes;
    public Material FlashMat;
    // [Space]
    public void HurtFlash (int times = 1, float delay = .1f) {
        StartCoroutine (HurtF (times, delay));
    }

    IEnumerator HurtF (int times = 1, float delay = .1f) {
        List<Material> OriginalMats = new List<Material> ();
        foreach (var mesh in Meshes) {
            OriginalMats.Add (mesh.material);
        }
        for (int t = times - 1; t >= 0; t--) {
            foreach (var mesh in Meshes) {
                mesh.material = FlashMat;
            }
            yield return new WaitForSeconds (delay);
            for (int m = Meshes.Length - 1; m >= 0; m--) {
                Meshes[m].material = OriginalMats[m];
            }
        }
    }
}