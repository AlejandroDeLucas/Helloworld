using UnityEngine;
using UnityEngine.UI;

namespace TinyHunter.UI.Panels
{
    public class DebugChecklistUI : MonoBehaviour
    {
        [SerializeField] private Text checklistText;
        [SerializeField] private bool questAccepted;
        [SerializeField] private bool enteredHunt;
        [SerializeField] private bool killedTarget;
        [SerializeField] private bool pickedLoot;
        [SerializeField] private bool returnedHub;
        [SerializeField] private bool craftedItem;
        [SerializeField] private bool equippedItem;
        [SerializeField] private bool savedGame;

        private void Start() => Refresh();

        public void SetQuestAccepted() { questAccepted = true; Refresh(); }
        public void SetEnteredHunt() { enteredHunt = true; Refresh(); }
        public void SetKilledTarget() { killedTarget = true; Refresh(); }
        public void SetPickedLoot() { pickedLoot = true; Refresh(); }
        public void SetReturnedHub() { returnedHub = true; Refresh(); }
        public void SetCraftedItem() { craftedItem = true; Refresh(); }
        public void SetEquippedItem() { equippedItem = true; Refresh(); }
        public void SetSavedGame() { savedGame = true; Refresh(); }

        private void Refresh()
        {
            if (checklistText == null) return;
            checklistText.text =
                $"[ {(questAccepted ? 'x' : ' ')} ] Accepted Quest\n" +
                $"[ {(enteredHunt ? 'x' : ' ')} ] Entered Hunt\n" +
                $"[ {(killedTarget ? 'x' : ' ')} ] Killed Target\n" +
                $"[ {(pickedLoot ? 'x' : ' ')} ] Picked Loot\n" +
                $"[ {(returnedHub ? 'x' : ' ')} ] Returned to Hub\n" +
                $"[ {(craftedItem ? 'x' : ' ')} ] Crafted Item\n" +
                $"[ {(equippedItem ? 'x' : ' ')} ] Equipped Item\n" +
                $"[ {(savedGame ? 'x' : ' ')} ] Saved Game";
        }
    }
}
