﻿using FreeLoader.GameLogic.Units;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.TestTools;
using NSubstitute;
using UnityEngine;
using FreeLoader.GameLogic.UI;
using UnityEngine.UI;
using UnityEngine.Events;
using FreeLoader.GameLogic.Player;
using FreeLoader.Interfaces;

namespace FreeLoader
{
    // Class
    class ShipMovementTests
    {
        // Constructor
        public class HandleMovement : TestsBase
        {
            private ShipMovement _sut;
            private IHealth _health;
            private IFuel _fuel;
            private GameObject _gameObject;
            private IRigidbody2D _mockedRigidBody;

            [OneTimeSetUp]
            protected void OneTimeSetup()
            {
                base.OneTimeSetup();

                _fuel = Substitute.For<IFuel>();
                _health = Substitute.For<IHealth>();
            }

            [SetUp]
            protected void BeforeEach()
            {
                _gameObject = new GameObject("Mocked", typeof(Rigidbody2D));
                _gameObject.SetActive(true);

                _mockedRigidBody = Substitute.For<IRigidbody2D>();
                
                InitHealthMock();
                InitFuelMock();

                _sut = new ShipMovement(_gameObject.transform, _mockedRigidBody, _health, _fuel);

                _fuel.ClearReceivedCalls();
            }

            [Test]
            public void It_should_rotate_ship_right_on_horizontal_right_movement()
            {
                // Arrange
                var verticalMovement = 0f;
                var horizontalMovement = 100.0f;

                // Act
                _sut.HandleMovement(horizontalMovement, verticalMovement);

                // Assert
                _mockedRigidBody.Received().AddTorque(Arg.Is<float>(x => x < 0));
            }

            [Test]
            public void It_should_rotate_ship_left_on_horizontal_left_movement()
            {
                // Arrange
                var verticalMovement = 0f;
                var horizontalMovement = -100.0f;

                // Act
                _sut.HandleMovement(horizontalMovement, verticalMovement);

                // Assert
                _mockedRigidBody.Received().AddTorque(Arg.Is<float>(x => x > 0));
            }

            [Test]
            public void It_should_accelerate_ship_on_positive_vertical_movement()
            {
                // Arrange
                var verticalMovement = 100f;
                var horizontalMovement = 0f;

                // Act
                _sut.HandleMovement(horizontalMovement, verticalMovement);

                // Assert
                _mockedRigidBody.Received().AddForce(Arg.Is<Vector2>(x => x.y > 0));
            }

            [Test]
            public void It_should_NOT_accelerate_ship_on_negative_vertical_movement()
            {
                // Arrange
                var verticalMovement = -100f;
                var horizontalMovement = 0f;

                // Act
                _sut.HandleMovement(horizontalMovement, verticalMovement);

                // Assert
                _mockedRigidBody.DidNotReceive().AddForce(Arg.Any<Vector2>());
            }

            [Test]
            public void It_should_combust_fuel_on_vertical_positive_movement()
            {
                // Arrange
                var verticalMovement = 100f;
                var horizontalMovement = 0f;

                // Act
                _sut.HandleMovement(horizontalMovement, verticalMovement);

                // Assert
                _fuel.Received().CombustFuel(Arg.Is<float>(0.01f));
            }

            [Test]
            public void It_should_NOT_combust_fuel_on_vertical_negative_movement()
            {
                // Arrange
                var verticalMovement = -100f;
                var horizontalMovement = 0f;

                // Act
                _sut.HandleMovement(horizontalMovement, verticalMovement);

                // Assert
                _fuel.DidNotReceive().CombustFuel(Arg.Any<float>());
            }

            [Test]
            public void It_should_combust_fuel_on_horizontal_positive_movement()
            {
                // Arrange
                var verticalMovement = 0;
                var horizontalMovement = 100f;

                // Act
                _sut.HandleMovement(horizontalMovement, verticalMovement);

                // Assert
                _fuel.Received().CombustFuel(Arg.Is<float>(0.01f));
            }

            [Test]
            public void It_should_combust_fuel_on_horizontal_negative_movement()
            {
                // Arrange
                var verticalMovement = 0;
                var horizontalMovement = -100f;

                // Act
                _sut.HandleMovement(horizontalMovement, verticalMovement);

                // Assert
                _fuel.Received().CombustFuel(Arg.Is<float>(0.01f));
            }

            [Test]
            public void It_should_NOT_accelerate_ship_if_there_is_no_fuel()
            {
                // Arrange
                _fuel.CurrentFuel = 0;
                _fuel.IsOutOfFuel.Returns(true);
                var verticalMovement = 100f;
                var horizontalMovement = 100f;

                // Act
                _sut.HandleMovement(horizontalMovement, verticalMovement);

                // Assert
                _mockedRigidBody.DidNotReceive().AddForce(Arg.Any<Vector2>());
            }

            [Test]
            public void It_should_NOT_rotate_ship_if_there_is_no_fuel()
            {
                // Arrange
                _fuel.CurrentFuel = 0;
                _fuel.IsOutOfFuel.Returns(true);
                var verticalMovement = 100f;
                var horizontalMovement = 100f;

                // Act
                _sut.HandleMovement(horizontalMovement, verticalMovement);

                // Assert
                _mockedRigidBody.DidNotReceive().AddTorque(Arg.Any<float>());
            }

            [Test]
            public void It_should_NOT_combust_fuel_if_there_is_no_fuel()
            {
                // Arrange
                _fuel.CurrentFuel = 0;
                _fuel.IsOutOfFuel.Returns(true);
                var verticalMovement = 100f;
                var horizontalMovement = -100f;

                // Act
                _sut.HandleMovement(horizontalMovement, verticalMovement);

                // Assert
                _fuel.DidNotReceive().CombustFuel(Arg.Any<float>());
            }



            private void InitFuelMock()
            {
                _fuel.CurrentFuel = 100;
                _fuel.IsOutOfFuel.Returns(false);
            }

            private void InitHealthMock()
            {
                _health.CurrentHealth = 100;
                _health.IsAlive.Returns(true);
            }
        }
    }
}
