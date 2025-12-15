using WebVella.Tefter.MessagingEx;

namespace WebVella.Tefter.Tests.Services;

[Collection("TfEventBusTests")]
public partial class TfEventBusTests : BaseTest
{
	public class TestEvent : ITfEventPayload { public int Value { get; set; } }
	public class LoginEvent : ITfEventPayload { public string Username { get; set; } }
	public class ClickEvent : ITfEventPayload { public int X { get; set; } }
	public class SystemEvent : ITfEventPayload { public DateTime Timestamp { get; set; } }
	public class InheritedLoginEvent : LoginEvent { public string DeviceId { get; set; } }
	public class InvalidPayload { } // does not implement ITfEventPayload

	
	private readonly ITfEventBus _bus;

	public TfEventBusTests() : base()
	{
		Assert.NotNull(ServiceProvider);

		_bus = ServiceProvider.GetService<ITfEventBus>();

		Assert.NotNull(_bus);
	}
	
	[Fact]
	public void TfEventHubTests_Subscribe_ThrowsException_WhenTypeDoesNotImplementITfEventPayload()
	{
		using (locker.Lock())
		{
			Assert.Throws<ArgumentException>(() =>
			{
				_bus.Subscribe<InvalidPayload>(_ => { });
			});
		}
	}

	[Fact]
	public async Task TfEventBusTests_Subscribe_ThrowsException_WhenTypeDoesNotImplementITfEventPayload_Async()
	{
		using (await locker.LockAsync())
		{
			await Assert.ThrowsAsync<ArgumentException>(async () =>
			{
				await _bus.SubscribeAsync<InvalidPayload>(_ => { });
			});
		}
	}

	[Fact]
	public void TfEventBusTests_Publish_KeyedSubscription_OnlyReceivesMatchingKey()
	{
		using (locker.Lock())
		{
			var mockHandler = new Mock<Action<LoginEvent>>();

			IDisposable subscription = _bus.Subscribe<LoginEvent>(mockHandler.Object, "USER_LOGIN");

			_bus.Publish("USER_LOGIN", payload: new LoginEvent());

			_bus.Publish("OTHER_KEY", payload: new LoginEvent());

			_bus.Publish(payload: new LoginEvent());

			mockHandler.Verify(h => h(It.IsAny<LoginEvent>()), Times.Once);

			subscription.Dispose();
		}
	}

	[Fact]
	public void TfEventBusTests_Publish_GlobalSubscription_ReceivesAllKeys()
	{
		using (locker.Lock())
		{
			var mockHandler = new Mock<Action<LoginEvent>>();

			var subscription = _bus.Subscribe<LoginEvent>(mockHandler.Object, null);

			_bus.Publish("USER_LOGIN", payload: new LoginEvent());

			_bus.Publish(payload: new LoginEvent());

			mockHandler.Verify(h => h(It.IsAny<LoginEvent>()), Times.Exactly(2));

			subscription.Dispose();
		}
	}

	[Fact]
	public async Task TfEventBusTests_Publish_GlobalSubscription_ReceivesAllKeysAsync()
	{
		using (await locker.LockAsync())
		{
			var mockHandler = new Mock<Action<LoginEvent>>();

			var subscription = await _bus.SubscribeAsync<LoginEvent>(mockHandler.Object, null);

			await _bus.PublishAsync("USER_LOGIN", payload: new LoginEvent());

			await _bus.PublishAsync(payload: new LoginEvent());

			mockHandler.Verify(h => h(It.IsAny<LoginEvent>()), Times.Exactly(2));
			
			await subscription.DisposeAsync();
		}
	}


	[Fact]
	public void TfEventBusTests_Publish_SpecificTypeSubscription_ReceivesInheritedEvents()
	{
		using (locker.Lock())
		{
			var mockHandler = new Mock<Action<LoginEvent>>();

			var subscription = _bus.Subscribe<LoginEvent>(mockHandler.Object, null);

			_bus.Publish(payload: new LoginEvent());

			_bus.Publish(payload: new InheritedLoginEvent());

			mockHandler.Verify(h => h(It.IsAny<LoginEvent>()), Times.Exactly(2));
			
			subscription.Dispose();
		}
	}

	[Fact]
	public async Task TfEventBusTests_Publish_SpecificTypeSubscription_ReceivesInheritedEventsASync()
	{
		using (await locker.LockAsync())
		{
			var mockHandler = new Mock<Action<LoginEvent>>();

			var subscription = await _bus.SubscribeAsync<LoginEvent>(handler: mockHandler.Object);

			await _bus.PublishAsync(payload: new LoginEvent());

			await _bus.PublishAsync(payload: new InheritedLoginEvent());

			mockHandler.Verify(h => h(It.IsAny<LoginEvent>()), Times.Exactly(2));
			
			await subscription.DisposeAsync();
		}
	}

	[Fact]
	public void TfEventBusTests_Publish_InterfaceSubscription_ReceivesAllPayloads()
	{
		using (locker.Lock())
		{
			var mockHandler = new Mock<Action<ITfEventPayload>>();

			var subscription = _bus.Subscribe<ITfEventPayload>(handler: mockHandler.Object);

			_bus.Publish(payload: new LoginEvent());
			_bus.Publish(payload: new ClickEvent());
			_bus.Publish(payload: new SystemEvent());

			mockHandler.Verify(h => h(It.IsAny<ITfEventPayload>()), Times.Exactly(3));

			subscription.Dispose();
		}
	}

	[Fact]
	public async Task TfEventBusTests_Publish_InterfaceSubscription_ReceivesAllPayloadsAsync()
	{
		using (await locker.LockAsync())
		{
			var mockHandler = new Mock<Action<ITfEventPayload>>();

			var subscription = await _bus.SubscribeAsync<ITfEventPayload>(handler: mockHandler.Object);

			await _bus.PublishAsync(payload: new LoginEvent());
			await _bus.PublishAsync(payload: new ClickEvent());
			await _bus.PublishAsync(payload: new SystemEvent());

			mockHandler.Verify(h => h(It.IsAny<ITfEventPayload>()), Times.Exactly(3));

			await subscription.DisposeAsync();
		}
	}

	[Fact]
	public void TfEventBusTests_Publish_NullPayload_CallsHandlerWithNull()
	{
		using (locker.Lock())
		{
			ITfEventPayload? receivedPayload = new LoginEvent();

			var subscription = _bus.Subscribe<LoginEvent>(p => receivedPayload = p);

			_bus.Publish();

			Assert.Null(receivedPayload);

			subscription.Dispose();
		}
	}

	[Fact]
	public async Task TfEventBusTests_Publish_NullPayload_CallsHandlerWithNullAsync()
	{
		using (await locker.LockAsync())
		{
			ITfEventPayload? receivedPayload = new LoginEvent();

			var subscription = await _bus.SubscribeAsync<LoginEvent>(p => receivedPayload = p);

			await _bus.PublishAsync();

			Assert.Null(receivedPayload);

			await subscription.DisposeAsync();
		}
	}

	[Fact]
	public async Task TfEventBusTests_Subscribe_WithAsyncHandler_AddsSubscription_AndIDisposableRemovesIt()
	{
		using (await locker.LockAsync())
		{
			var callCount = 0;
			Func<TestEvent?, ValueTask> handler = (e) =>
			{
				callCount++;
				return ValueTask.CompletedTask;
			};

			IDisposable subscription = _bus.Subscribe<TestEvent>(handler);

			Assert.Equal(1, GetSubscriptionCount());
			await _bus.PublishAsync(payload: new TestEvent());
			Assert.Equal(1, callCount);

			subscription.Dispose();

			Assert.Equal(0, GetSubscriptionCount());
			await _bus.PublishAsync(payload: new TestEvent());
			Assert.Equal(1, callCount);
		}
	}

	[Fact]
	public async Task TfEventBusTests_Subscribe_WithAsyncHandlerAndKey_AddsCorrectly()
	{
		using (await locker.LockAsync())
		{
			var key = "SpecificKey";
			var called = false;
			Func<TestEvent?, ValueTask> handler = (e) =>
			{
				called = true;
				return ValueTask.CompletedTask;
			};

			using (var subscription = _bus.Subscribe<TestEvent>(handler, key))
			{
				await _bus.PublishAsync(key: key, payload: new TestEvent());
				Assert.True(called);

				called = false;
				_bus.PublishAsync(key: "OtherKey", payload: new TestEvent());
				Assert.False(called);
			}
		}
	}


	[Fact]
	public void TfEventBusTests_Subscribe_IDisposable_RemovesSubscriptionOnDispose()
	{
		using (locker.Lock())
		{
			var mockHandler = new Mock<Action<LoginEvent>>();

			var subscription = _bus.Subscribe<LoginEvent>(handler: mockHandler.Object);

			_bus.Publish(payload: new LoginEvent());

			subscription.Dispose();

			_bus.Publish(payload: new LoginEvent());

			mockHandler.Verify(h => h(It.IsAny<LoginEvent>()), Times.Once);
		}
	}

	[Fact]
	public async Task TfEventHubTests_SubscribeAsync_IAsyncDisposable_RemovesSubscriptionOnDisposeAsync()
	{
		using (await locker.LockAsync())
		{
			var mockHandler = new Mock<Action<LoginEvent>>();

			IAsyncDisposable? subscription = await _bus.SubscribeAsync<LoginEvent>(handler: mockHandler.Object);

			await _bus.PublishAsync(payload: new LoginEvent());

			await subscription.DisposeAsync();

			await _bus.PublishAsync(payload: new LoginEvent());

			mockHandler.Verify(h => h(It.IsAny<LoginEvent>()), Times.Once);
		}
	}

	private int GetSubscriptionCount()
	{
		var field = typeof(TfEventBus).GetField("_subscribers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		var subscribers = field?.GetValue(_bus) as System.Collections.IList;
		return subscribers?.Count ?? 0;
	}
}