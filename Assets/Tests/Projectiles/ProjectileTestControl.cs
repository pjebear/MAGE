using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ProjectileTestControl : MonoBehaviour
{
    ProjectileDirector ProjectileDirector;
    List<Tests.Projectiles.TestCase> mTests = new List<Tests.Projectiles.TestCase>();

    private void Awake()
    {
        ProjectileDirector = GetComponent<ProjectileDirector>();
        ProjectileDirector.Init();
    }

    public void Run()
    {
        foreach (Tests.Projectiles.TestCase testCase in GetComponentsInChildren<Tests.Projectiles.TestCase>())
        {
            testCase.Setup(ProjectileDirector);
            testCase.Run();
        }
    }
}

