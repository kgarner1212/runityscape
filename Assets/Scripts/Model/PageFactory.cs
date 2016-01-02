﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class PageFactory {
    public static Page createPage(string text, PageType pageType, List<Character> left, List<Character> right,
        Process onFirstEnter, Process onEnter, Process onFirstExit, Process onExit,
        List<Process> actions) {
        Page page = new Page();
        page.setText(text);
        page.setPageType(pageType);
        page.setLeft(left);
        page.setRight(right);
        page.setActions(actions);
        return page;
    }
}
