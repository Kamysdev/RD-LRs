using UnityEngine;

namespace LR6.Dialogues
{
    public static class ExampleDialogues
    {
        public const string NpcDialogue = @"<messages uid=""9"">
  <message mid=""1"">Привет, герой. Я могу сделать тебя убедительнее.</message>
  <answers>
    <answer auid=""2"" muid=""3"" action=""none"">Расскажи о себе.</answer>
    <answer auid=""3"" muid=""-1"" action=""stat_inc:charisma:1;dialogue end"">Хочу стать харизматичнее.</answer>
  </answers>
  </message>
  <message mid=""3"">Твоя харизма выросла. Возвращайся, если захочешь продолжить.</message>
  <answers>
    <answer auid=""4"" muid=""-1"" action=""dialogue end"">Спасибо.</answer>
  </answers>
  </message>
</messages>";

        public const string SecondNpcDialogue = @"<messages uid=""12"">
  <message mid=""10"">Я тренирую либо силу, либо интеллект. Что тебе нужнее?</message>
  <answers>
    <answer auid=""11"" muid=""-1"" action=""stat_inc:strength:1;dialogue end"">Нужна сила.</answer>
    <answer auid=""12"" muid=""-1"" action=""stat_inc:intelligence:1;dialogue end"">Нужен интеллект.</answer>
  </answers>
  </message>
</messages>";

        public const string DoorDialogue = @"<messages uid=""25"">
  <message mid=""20"">Перед тобой закрытая дверь. Как попробуешь ее открыть?</message>
  <answers>
    <answer auid=""21"" muid=""-1"" action=""stat_check:strength:>=:2;door open;dialogue end"">[Выбить дверь]</answer>
    <answer auid=""22"" muid=""-1"" action=""stat_check:intelligence:>=:2;door open;dialogue end"">[Повернуть ручку]</answer>
    <answer auid=""23"" muid=""-1"" action=""dialogue end"">Отойти.</answer>
  </answers>
  </message>
</messages>";
    }
}
