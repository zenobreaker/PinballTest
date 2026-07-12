using UnityEngine;

public class ClusterBall : Ball
{
    [Header("파편 설정")]
    [SerializeField] private string fragmentPoolTag = "ClusterFragment"; // 파편 프리팹 풀링 이름
    [SerializeField] private int fragmentCount = 3; // 쪼개질 개수

    protected override void ApplySpecialEffect(GameObject target)
    {
        if (skillData == null) return;

        // effectChance: 파편 생성 확률 (40%, 50%, 60%)
        if (Random.value <= skillData.effectChance)
        {
            for (int i = 0; i < fragmentCount; i++)
            {
                // 약간 퍼지는 무작위 방향 설정
                Vector2 randomDir = Random.insideUnitCircle.normalized;

                // 오브젝트 풀에서 파편 볼 꺼내오기
                ObjectPooler.DeferredSpawnWithCallback(fragmentPoolTag, transform, (obj) =>
                {
                    ObjectPooler.FinishSpawn(obj);
                    if (obj.TryGetComponent<Ball>(out Ball fragmentBall))
                    {
                        // 파편 데미지는 effectValue (10, 15, 20)로 덮어씌움
                        // 파편용 더미 SkillData를 만들어서 넘겨줍니다.
                        SkillLevelData fragData = new SkillLevelData { effectValue = skillData.effectValue };
                        fragmentBall.SetSkillData(fragData);
                        BallData ballData = new BallData();
                        ballData.ballType = BallType.Normal;
                        ballData.damageData = new();
                        ballData.damageData.baseDamage = fragData.effectValue;

                        fragmentBall.SetUpData(owner, ballData, runtimeData);

                        // 발사! 
                        fragmentBall.Launch(randomDir, currentSpeed);
                    }
                });
            }
        }
    }
}