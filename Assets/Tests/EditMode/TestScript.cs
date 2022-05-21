using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestScript
{
    // A Test behaves as an ordinary method
    [Test]
    public void Test_CheckRefillingEmptyTiles()
    {
        // Assign
        var board = new GameObject().AddComponent<Board>();

        // Act
        //board.RefillEmptyTiles();

        // Assert
        Assert.Throws<Exception>(() => board.RefillEmptyTiles());
    }

    [Test]
    public void Test_FindMatches() 
    {
        var board = new GameObject().AddComponent<Board>();
        //Assert.
    }
   
}
