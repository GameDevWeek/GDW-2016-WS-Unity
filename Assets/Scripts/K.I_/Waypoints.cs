using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.K.I_ {

    [System.Serializable]
    public struct Pair {
        public int First, Second;

        public sealed class FirstSecondEqualityComparer : IEqualityComparer<Pair> {
            public bool Equals(Pair x, Pair y) {
                return x.First == y.First && x.Second == y.Second;
            }

            public int GetHashCode(Pair obj) {
                unchecked {
                    return (obj.First * 397) ^ obj.Second;
                }
            }
        }
    }


    [CreateAssetMenu()]
    public class Waypoints : ScriptableObject {

        public Vector3[] points;
        public Pair[] pairs;


        public Vector3 GetNextPoint(int currentIndex, out int nextIndex) {
            var sel = pairs.Where(pairs => pairs.First == currentIndex).ToArray();

            int i = Random.Range(0, sel.Length);

            nextIndex = sel[i].Second;
            return points[nextIndex];
        }


    }
}
