using WebVella.Tefter.MessagingEx;

namespace WebVella.Tefter.Tests.Messaging;

public partial class TfEventBusTests : BaseTest
{
	public class TestEvent : ITfEventArgs {}
	public class LoginEvent : ITfEventArgs {}
	public class ClickEvent : ITfEventArgs {}
	public class SystemEvent : ITfEventArgs {}
	public class InheritedLoginEvent : LoginEvent {}
	public class InvalidEventArgs { } // does not implement ITfEventArgs


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
				_bus.Subscribe<InvalidEventArgs>((_, _) => { });
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
				await _bus.SubscribeAsync<InvalidEventArgs>((_, _) => { });
			});
		}
	}

	[Fact]
	public void TfEventBusTests_Publish_KeyedSubscription_OnlyReceivesMatchingKey()
	{
		using (locker.Lock())
		{
			var mockHandler = new Mock<Action<string?, LoginEvent>>();

			IDisposable subscription = _bus.Subscribe<LoginEvent>(handler: mockHandler.Object, key: "USER_LOGIN");

			_bus.Publish(key: "USER_LOGIN", args: new LoginEvent());

			_bus.Publish(key: "OTHER_KEY", args: new LoginEvent());

			_bus.Publish(args: new LoginEvent());

			mockHandler.Verify(
				h => h(
					It.Is<string?>(key => true), 
					It.IsAny<LoginEvent>()                     
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
			var mockHandler = new Mock<Action<string?,LoginEvent>>();

			var subscription = _bus.Subscribe<LoginEvent>(handler: mockHandler.Object, key: null);

			_bus.Publish(key: "USER_LOGIN", args: new LoginEvent());

			_bus.Publish(args: new LoginEvent());

			mockHandler.Verify(
				h => h(
					It.Is<string?>(key => true),
					It.IsAny<LoginEvent>()
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
			var mockHandler = new Mock<Action<string?,LoginEvent>>();

			var subscription = await _bus.SubscribeAsync<LoginEvent>(handler: mockHandler.Object, key: null);

			await _bus.PublishAsync(key: "USER_LOGIN", args: new LoginEvent());

			await _bus.PublishAsync(args: new LoginEvent());

			mockHandler.Verify(
				h => h(
					It.Is<string?>(key => true),
					It.IsAny<LoginEvent>()
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
			var mockHandler = new Mock<Action<string?,LoginEvent>>();

			var subscription = _bus.Subscribe<LoginEvent>(handler: mockHandler.Object, key: null);

			_bus.Publish(args: new LoginEvent());

			_bus.Publish(args: new InheritedLoginEvent());

			mockHandler.Verify(
				h => h(
					It.Is<string?>(key => true),
					It.IsAny<LoginEvent>()
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
			var mockHandler = new Mock<Action<string?,LoginEvent>>();

			var subscription = await _bus.SubscribeAsync<LoginEvent>(handler: mockHandler.Object);

			await _bus.PublishAsync(args: new LoginEvent());

			await _bus.PublishAsync(args: new InheritedLoginEvent());

			mockHandler.Verify(
				h => h(
					It.Is<string?>(key => true),
					It.IsAny<LoginEvent>()
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
			var mockHandler = new Mock<Action<string?,ITfEventArgs>>();

			var subscription = _bus.Subscribe<ITfEventArgs>(handler: mockHandler.Object);

			_bus.Publish(args: new LoginEvent());
			_bus.Publish(args: new ClickEvent());
			_bus.Publish(args: new SystemEvent());

			mockHandler.Verify(
				h => h(
					It.Is<string?>(key => true),
					It.IsAny<ITfEventArgs>()
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
			var mockHandler = new Mock<Action<string?,ITfEventArgs>>();

			var subscription = await _bus.SubscribeAsync<ITfEventArgs>(handler: mockHandler.Object);

			await _bus.PublishAsync(args: new LoginEvent());
			await _bus.PublishAsync(args: new ClickEvent());
			await _bus.PublishAsync(args: new SystemEvent());

			mockHandler.Verify(
				h => h(
					It.Is<string?>(key => true),
					It.IsAny<ITfEventArgs>()
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
			ITfEventArgs? receivedArgs = new LoginEvent();

			var subscription = _bus.Subscribe<LoginEvent>((key,args) => { receivedKey = key; receivedArgs = args; } );

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
			ITfEventArgs? receivedArgs = new LoginEvent();

			var subscription = await _bus.SubscribeAsync<LoginEvent>((key, args) => { receivedKey = key; receivedArgs = args; });

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
			Func<string?, TestEvent?, ValueTask> handler = (k,p) =>
			{
				callCount++;
				return ValueTask.CompletedTask;
			};

			IDisposable subscription = _bus.Subscribe<TestEvent>(handler);

			Assert.Equal(1, GetSubscriptionCount());
			await _bus.PublishAsync(args: new TestEvent());
			Assert.Equal(1, callCount);

			subscription.Dispose();

			Assert.Equal(0, GetSubscriptionCount());
			await _bus.PublishAsync(args: new TestEvent());
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
			Func<string?,TestEvent?, ValueTask> handler = (k,p) =>
			{
				called = true;
				return ValueTask.CompletedTask;
			};

			using (var subscription = _bus.Subscribe<TestEvent>(handler, key))
			{
				await _bus.PublishAsync(key: key, args: new TestEvent());
				Assert.True(called);

				called = false;
				await _bus.PublishAsync(key: "OtherKey", args: new TestEvent());
				Assert.False(called);
			}
		}
	}


	[Fact]
	public void TfEventBusTests_Subscribe_IDisposable_RemovesSubscriptionOnDispose()
	{
		using (locker.Lock())
		{
			var mockHandler = new Mock<Action<string?,LoginEvent>>();

			var subscription = _bus.Subscribe<LoginEvent>(handler: mockHandler.Object);

			_bus.Publish(args: new LoginEvent());

			subscription.Dispose();

			_bus.Publish(args: new LoginEvent());

			mockHandler.Verify(
				h => h(
					It.Is<string?>(key => true),
					It.IsAny<LoginEvent>()
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
			var mockHandler = new Mock<Action<string?,LoginEvent>>();

			IAsyncDisposable? subscription = await _bus.SubscribeAsync<LoginEvent>(handler: mockHandler.Object);

			await _bus.PublishAsync(args: new LoginEvent());

			await subscription.DisposeAsync();

			await _bus.PublishAsync(args: new LoginEvent());

			mockHandler.Verify(
				h => h(
					It.Is<string?>(key => true),
					It.IsAny<LoginEvent>()
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