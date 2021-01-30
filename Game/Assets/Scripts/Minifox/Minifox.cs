using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Minifox : MonoBehaviour
{
    [SerializeField] protected PlayerHud m_playerHud;
    [SerializeField] protected string m_upgradeText;

    protected string m_baseText = "Yahaha! You found me!";
    protected string m_remainingFoxesText = "There are {0} foxes left. Find us all and you will defeat the evil more easily.";
    protected string m_foundAll = "You have found us all! You are now perfectly prepared for the last fight.";

    protected string GetBaseDialogueText()
    {
        var activeCount = FindObjectsOfType<Minifox>().Length - 1;
        if (activeCount > 0)
        {
            return string.Format(this.m_baseText + " " + this.m_remainingFoxesText, activeCount);
        }

        return this.m_baseText + " " + this.m_foundAll;
    }

    public Upgrade Upgrade { get; protected set; }

    public void ShowDialogue()
    {
        this.m_playerHud.ShowDialogue(this.GetBaseDialogueText() + " " + this.m_upgradeText);
    }
}
