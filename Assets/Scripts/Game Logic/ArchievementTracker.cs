using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;


public class ArchievementTracker : Singleton<ArchievementTracker> {
    [SerializeField] private List<Archievement> archievements = new List<Archievement>();
    private List<Invoker> invokers = new List<Invoker>();

    class Invoker {
        protected Action invokable;

        public Invoker(Action invokable) {
            this.invokable = invokable;
        }

        public void Invoke() {
            invokable.Invoke();
        }
    }

    class Invoker<T> : Invoker {
        private new Action<T> invokable;

        public Invoker(Action<T> invokable) : base(null) {
            this.invokable = invokable;
        }

        public void Invoke(T param) {
            invokable.Invoke(param);
        }
    }

    [Preserve]
    void Start() {
        foreach (var archievement in archievements) {
            if(archievement.saveOnLocalMachine)
                archievement.currentValue = PlayerPrefs.GetFloat(archievement.name, archievement.defaultValue);

            var type = Type.GetType(archievement.assembly);
            if(type == null) throw new Exception("can't find reference to event containing class. this should'nt happen...");
            var eventinfo = type.GetEvent(archievement.property.Name, BindingFlags.Public | BindingFlags.Static);
            if (eventinfo == null) {
                throw new Exception("could not find event in class " + archievement.classPath);
            }

            if (archievement.increment_instead_set) {
                Action del = () => {
                    UpdateArchievement(archievement.name, archievement.currentValue + archievement.scaling);
                };

                var inv = new Invoker(del);
                invokers.Add(inv);
                var methodinfo = inv.GetType().GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance);

                eventinfo.AddEventHandler(null, Delegate.CreateDelegate(typeof(Action), inv, methodinfo));
            }else {

                Action<float> del = (float f) => {
                    UpdateArchievement(archievement.name, f / archievement.scaling);
                };

                var inv = new Invoker<float>(del);
                invokers.Add(inv);
                var methodinfo = inv.GetType().GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance);

                eventinfo.AddEventHandler(null, Delegate.CreateDelegate(typeof(Action<float>), inv, methodinfo));

            }


        }
    }

    public void UpdateArchievement(string name, float value) {
        // Debug.LogAssertion(archievements.Any((a) => a.name == name));

        archievements.First((a) => a.name == name).currentValue = value;
    }

    public void UpdateArchievement(Archievement arch, float value) {
        // Debug.LogAssertion(archievements.Contains(arch));
        arch.currentValue = value;
    }

    public List<Archievement> getAchievment()
    {
        return archievements;
    }

    void OnDestroy() {
        foreach (var archievement in archievements) {
            if(! archievement.saveOnLocalMachine) continue;

            PlayerPrefs.SetFloat(archievement.name, archievement.currentValue);
        }

    }
}
