using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    public class CombatTurnSingletonRawComponent : EcsSingletonRawComponent
    {
        /// <summary>
        /// ��ǰ�ж���<see cref="Entity"/>
        /// </summary>
        public Entity CurrentEntity;
        /// <summary>
        /// �Ƿ��ܽ��ж���
        /// </summary>
        public bool HasAction;
        /// <summary>
        /// �Ƿ��ܽ��и�������
        /// </summary>
        public bool HasBonusAction;
        /// <summary>
        /// �������Ƿ���������غ�
        /// </summary>
        public bool EndTurn;

        /// <summary>
        /// �����µ�һ�غϿ�ʼʱˢ�»غ�����
        /// </summary>
        /// <param name="actEntity"> �ûغ��ж���<see cref="Entity"/> </param>
        public void RefreshTurn(Entity actEntity)
        {
            CurrentEntity = actEntity;
            HasAction = true;
            HasBonusAction = true;
            EndTurn = false;
        }
    }
}
