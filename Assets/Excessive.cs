using System.Collections;
using System.Collections.Generic;
using TNet;
using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof(TNObject))]
    public class Excessive : MonoBehaviour
    {
        TNObject tno;
        private void Awake()
        {
            tno = GetComponent<TNObject>();
        }

        public void TakeOrders(Vector3 move, bool crouch, bool jump)
        {
            tno.Send("DODO", Target.OthersSaved, move, crouch, jump);
        }

        [RFC]
        protected void DODO(Vector3 move, bool crouch, bool jump)
        {
            //GetComponent<ThirdPersonCharacter>().Move(move, crouch, jump);
        }
    }
}
public class mytest
{
    UnityStandardAssets.Characters.ThirdPerson.Excessive ex;
}