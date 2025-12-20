using System.Collections.Generic;
using UnityEngine;

public class RouteCalculator : MonoBehaviour
{
    // Waypoint sayýsý 12 olmalý (P1..P12)
    public Transform patrolPointsParent;

    [Header("Random movement")]
    [Range(0f, 1f)] public float preferSameColumn = 0.55f; // %55 sütun, %45 satýr gibi
    public bool avoidImmediateBacktrack = true;            // 1->4->1 gibi hemen geri dönmesin

    Transform[] points;

    int current1Based = 1; // 1..12
    int last1Based = -1;

    void Awake()
    {
        // Parent altýndan P1..P12 topla
        int count = patrolPointsParent.childCount;
        points = new Transform[count];
        for (int i = 0; i < count; i++) points[i] = patrolPointsParent.GetChild(i);

        // Ýsim sýrasý: P1,P2,... (önemli)
        System.Array.Sort(points, (a, b) => a.name.CompareTo(b.name));

        if (points.Length != 12)
            Debug.LogWarning($"Expected 12 patrol points, got {points.Length}.");
    }

    // Dýþarýdan çaðýr: "bir sonraki waypoint hangi index?"
    public int PickNextPointIndex0Based()
    {
        int next1 = PickNextIndex1Based(current1Based, last1Based);
        last1Based = current1Based;
        current1Based = next1;
        return next1 - 1; // 0-based
    }

    int PickNextIndex1Based(int current, int last)
    {
        int row = (current - 1) / 3; // 0..3
        int col = (current - 1) % 3; // 0..2

        // adaylar: ayný satýr + ayný sütun
        List<int> sameRow = new List<int>();
        List<int> sameCol = new List<int>();

        // ayný satýr: row sabit, col 0..2
        for (int c = 0; c < 3; c++)
        {
            int n = row * 3 + c + 1; // 1..12
            if (n != current) sameRow.Add(n);
        }

        // ayný sütun: col sabit, row 0..3
        for (int r = 0; r < 4; r++)
        {
            int n = r * 3 + col + 1; // 1..12
            if (n != current) sameCol.Add(n);
        }

        // hemen geri dönmeyi engellemek istersen
        if (avoidImmediateBacktrack && last > 0)
        {
            sameRow.Remove(last);
            sameCol.Remove(last);
        }

        // Eðer listeler boþ kaldýysa (çok nadir), backtrack'e izin ver
        if (sameRow.Count == 0)
            for (int c = 0; c < 3; c++) { int n = row * 3 + c + 1; if (n != current) sameRow.Add(n); }

        if (sameCol.Count == 0)
            for (int r = 0; r < 4; r++) { int n = r * 3 + col + 1; if (n != current) sameCol.Add(n); }

        // Random seçim: ya satýrdan ya sütundan
        bool chooseCol = Random.value < preferSameColumn;

        List<int> chosen = chooseCol ? sameCol : sameRow;
        return chosen[Random.Range(0, chosen.Count)];
    }

    // Ýstersen: current waypoint pozisyonu
    public Vector3 GetCurrentTargetPosition()
    {
        return points[current1Based - 1].position;
    }
}
