using WebVella.Tefter.MessagingEx;

namespace WebVella.Tefter.Tests.Messaging;

public partial class TfEventBusTests : BaseTest
{
	public class TestEventPayload : ITfEventPayload {}
	public class LoginEventPayload : ITfEventPayload {}
	public class ClickEventPayload : ITfEventPayload {}
	public class SystemEventPayload : ITfEventPayload {}
	public class InheritedLoginEventPayload : LoginEventPayload {}
	public class InvalidEventPayload { } // does not implement ITfEventPayload


	private readonly ITfEventBus _bus;

	public TfEventBusTests() : base()
	{
		Assert.NotNull(ServiceProvider);

		_bus = ServiceProvider.GetService<ITfEventBus>();

		Assert.NotNull(_bus);
	}

	[Fact]
	public void TfEventHubTests_Subscribe_ThrowsException_WhenTypeDoesNotImplementITfEventArgs()
	{
		using (locker.Lock())
		{
			Assert.Throws<ArgumentException>(() =>
			{
				_bus.Subscribe<InvalidEventPayload>((_, _) => { });
			});
		}
	}

	[Fact]
	public async Task TfEventBusTests_Subscribe_ThrowsException_WhenTypeDoesNotImplementITfEventArgs_Async()
	{
		using (await locker.LockAsync())
		{
			await Assert.ThrowsAsync<ArgumentException>(async () =>
			{
				await _bus.SubscribeAsync<InvalidEventPayload>((_, _) => { });
			});
		}
	}

	[Fact]
	public void TfEventBusTests_Publish_KeyedSubscription_OnlyReceivesMatchingKey()
	{
		using (locker.Lock())
		{
			var mockHandler = new Mock<Action<string?, LoginEventPayload>>();

			IDisposable subscription = _bus.Subscribe<LoginEventPayload>(handler: mockHandler.Object, key: "USER_LOGIN");

			_bus.Publish(key: "USER_LOGIN", args: new LoginEventPayload());

			_bus.Publish(key: "OTHER_KEY", args: new LoginEventPayload());

			_bus.Publish(args: new LoginEventPayload());

			mockHandler.Verify(
				h => h(
					It.Is<string?>(key => true), 
					It.IsAny<LoginEventPayload>()                     
				),
				Times.Once
			);

			subscription.Dispose();
		}
	}

	[Fact]
	public void TfEventBusTests_Publish_GlobalSubscription_ReceivesAllKeys()
	{
		using (locker.Lock())
		{
			var mockHandler = new Mock<Action<string?,LoginEventPayload>>();

			var subscription = _bus.Subscribe<LoginEventPayload>(handler: mockHandler.Object, key: null);

			_bus.Publish(key: "USER_LOGIN", args: new LoginEventPayload());

			_bus.Publish(args: new LoginEventPayload());

			mockHandler.Verify(
				h => h(
					It.Is<string?>(key => true),
					It.IsAny<LoginEventPayload>()
				),
				Times.Exactly(2)
			);

			subscription.Dispose();
		}
	}

	[Fact]
	public async Task TfEventBusTests_Publish_GlobalSubscription_ReceivesAllKeysAsync()
	{
		using (await locker.LockAsync())
		{
			var mockHandler = new Mock<Action<string?,LoginEventPayload>>();

			var subscription = await _bus.SubscribeAsync<LoginEventPayload>(handler: mockHandler.Object, key: null);

			await _bus.PublishAsync(key: "USER_LOGIN", args: new LoginEventPayload());

			await _bus.PublishAsync(args: new LoginEventPayload());

			mockHandler.Verify(
				h => h(
					It.Is<string?>(key => true),
					It.IsAny<LoginEventPayload>()
				),
				Times.Exactly(2)
			);

			await subscription.DisposeAsync();
		}
	}


	[Fact]
	public void TfEventBusTests_Publish_SpecificTypeSubscription_ReceivesInheritedEvents()
	{
		using (locker.Lock())
		{
			var mockHandler = new Mock<Action<string?,LoginEventPayload>>();

			var subscription = _bus.Subscribe<LoginEventPayload>(handler: mockHandler.Object, key: null);

			_bus.Publish(args: new LoginEventPayload());

			_bus.Publish(args: new InheritedLoginEventPayload());

			mockHandler.Verify(
				h => h(
					It.Is<string?>(key => true),
					It.IsAny<LoginEventPayload>()
				),
				Times.Exactly(2)
			);

			subscription.Dispose();
		}
	}

	[Fact]
	public async Task TfEventBusTests_Publish_SpecificTypeSubscription_ReceivesInheritedEventsASync()
	{
		using (await locker.LockAsync())
		{
			var mockHandler = new Mock<Action<string?,LoginEventPayload>>();

			var subscription = await _bus.SubscribeAsync<LoginEventPayload>(handler: mockHandler.Object);

			await _bus.PublishAsync(args: new LoginEventPayload());

			await _bus.PublishAsync(args: new InheritedLoginEventPayload());

			mockHandler.Verify(
				h => h(
					It.Is<string?>(key => true),
					It.IsAny<LoginEventPayload>()
				),
				Times.Exactly(2)
			);

			await subscription.DisposeAsync();
		}
	}

	[Fact]
	public void TfEventBusTests_Publish_InterfaceSubscription_ReceivesAllArgs()
	{
		using (locker.Lock())
		{
			var mockHandler = new Mock<Action<string?,ITfEventPayload>>();

			var subscription = _bus.Subscribe<ITfEventPayload>(handler: mockHandler.Object);

			_bus.Publish(args: new LoginEventPayload());
			_bus.Publish(args: new ClickEventPayload());
			_bus.Publish(args: new SystemEventPayload());

			mockHandler.Verify(
				h => h(
					It.Is<string?>(key => true),
					It.IsAny<ITfEventPayload>()
				),
				Times.Exactly(3)
			);

			subscription.Dispose();
		}
	}

	[Fact]
	public async Task TfEventBusTests_Publish_InterfaceSubscription_ReceivesAllArgsAsync()
	{
		using (await locker.LockAsync())
		{
			var mockHandler = new Mock<Action<string?,ITfEventPayload>>();

			var subscription = await _bus.SubscribeAsync<ITfEventPayload>(handler: mockHandler.Object);

			await _bus.PublishAsync(args: new LoginEventPayload());
			await _bus.PublishAsync(args: new ClickEventPayload());
			await _bus.PublishAsync(args: new SystemEventPayload());

			mockHandler.Verify(
				h => h(
					It.Is<string?>(key => true),
					It.IsAny<ITfEventPayload>()
				),
				Times.Exactly(3)
			);

			await subscription.DisposeAsync();
		}
	}

	[Fact]
	public void TfEventBusTests_Publish_NullArgs_CallsHandlerWithNull()
	{
		using (locker.Lock())
		{
			string? receivedKey = null;
			ITfEventPayload? receivedArgs = new LoginEventPayload();

			var subscription = _bus.Subscribe<LoginEventPayload>((key,args) => { receivedKey = key; receivedArgs = args; } );

			_bus.Publish();

			Assert.Null(receivedArgs);

			subscription.Dispose();
		}
	}

	[Fact]
	public async Task TfEventBusTests_Publish_NullArgs_CallsHandlerWithNullAsync()
	{
		using (await locker.LockAsync())
		{
			string? receivedKey = null;
			ITfEventPayload? receivedArgs = new LoginEventPayload();

			var subscription = await _bus.SubscribeAsync<LoginEventPayload>((key, args) => { receivedKey = key; receivedArgs = args; });

			await _bus.PublishAsync();

			Assert.Null(receivedArgs);

			await subscription.DisposeAsync();
		}
	}


	[Fact]
	public async Task TfEventBusTests_Subscribe_WithAsyncHandler_AddsSubscription_AndIDisposableRemovesIt()
	{
		using (await locker.LockAsync())
		{
			var callCount = 0;
			Func<string?, TestEventPayload?, ValueTask> handler = (k,p) =>
			{
				callCount++;
				return ValueTask.CompletedTask;
			};

			IDisposable subscription = _bus.Subscribe<TestEventPayload>(handler);

			Assert.Equal(1, GetSubscriptionCount());
			await _bus.PublishAsync(args: new TestEventPayload());
			Assert.Equal(1, callCount);

			subscription.Dispose();

			Assert.Equal(0, GetSubscriptionCount());
			await _bus.PublishAsync(args: new TestEventPayload());
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
			Func<string?,TestEventPayload?, ValueTask> handler = (k,p) =>
			{
				called = true;
				return ValueTask.CompletedTask;
			};

			using (var subscription = _bus.Subscribe<TestEventPayload>(handler, key))
			{
				await _bus.PublishAsync(key: key, args: new TestEventPayload());
				Assert.True(called);

				called = false;
				await _bus.PublishAsync(key: "OtherKey", args: new TestEventPayload());
				Assert.False(called);
			}
		}
	}


	[Fact]
	public void TfEventBusTests_Subscribe_IDisposable_RemovesSubscriptionOnDispose()
	{
		using (locker.Lock())
		{
			var mockHandler = new Mock<Action<string?,LoginEventPayload>>();

			var subscription = _bus.Subscribe<LoginEventPayload>(handler: mockHandler.Object);

			_bus.Publish(args: new LoginEventPayload());

			subscription.Dispose();

			_bus.Publish(args: new LoginEventPayload());

			mockHandler.Verify(
				h => h(
					It.Is<string?>(key => true),
					It.IsAny<LoginEventPayload>()
				),
				Times.Once
			);
		}
	}

	[Fact]
	public async Task TfEventHubTests_SubscribeAsync_IAsyncDisposable_RemovesSubscriptionOnDisposeAsync()
	{
		using (await locker.LockAsync())
		{
			var mockHandler = new Mock<Action<string?,LoginEventPayload>>();

			IAsyncDisposable? subscription = await _bus.SubscribeAsync<LoginEventPayload>(handler: mockHandler.Object);

			await _bus.PublishAsync(args: new LoginEventPayload());

			await subscription.DisposeAsync();

			await _bus.PublishAsync(args: new LoginEventPayload());

			mockHandler.Verify(
				h => h(
					It.Is<string?>(key => true),
					It.IsAny<LoginEventPayload>()
				),
				Times.Once
			);
		}
	}

	private int GetSubscriptionCount()
	{
		var field = typeof(TfEventBus).GetField("_subscribers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		var subscribers = field?.GetValue(_bus) as System.Collections.IList;
		return subscribers?.Count ?? 0;
	}
}