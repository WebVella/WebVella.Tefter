using WebVella.Tefter.UI.EventsBus;

namespace WebVella.Tefter.Tests.Messaging;

public partial class TfEventBusTests : BaseTest
{
	public class TestUiEventPayload : ITfEventPayload {}
	public class LoginUiEventPayload : ITfEventPayload {}
	public class ClickUiEventPayload : ITfEventPayload {}
	public class SystemUiEventPayload : ITfEventPayload {}
	public class InheritedLoginUiEventPayload : LoginUiEventPayload {}
	public class InvalidEventPayload { } // does not implement ITfEventPayload


	private readonly ITfEventBus _bus;

	public TfEventBusTests() : base()
	{
		Assert.NotNull(ServiceProvider);

		_bus = ServiceProvider.GetService<ITfEventBus>()!;

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
			var mockHandler = new Mock<Action<string?, LoginUiEventPayload>>();

			IDisposable subscription = _bus.Subscribe<LoginUiEventPayload>(handler: mockHandler.Object!, key: "USER_LOGIN");

			_bus.Publish(key: "USER_LOGIN", payload: new LoginUiEventPayload());

			_bus.Publish(key: "OTHER_KEY", payload: new LoginUiEventPayload());

			_bus.Publish(payload: new LoginUiEventPayload());

			mockHandler.Verify(
				h => h(
					It.Is<string?>(key => true), 
					It.IsAny<LoginUiEventPayload>()                     
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
			var mockHandler = new Mock<Action<string?,LoginUiEventPayload>>();

			var subscription = _bus.Subscribe<LoginUiEventPayload>(handler: mockHandler.Object!, key: null);

			_bus.Publish(key: "USER_LOGIN", payload: new LoginUiEventPayload());

			_bus.Publish(payload: new LoginUiEventPayload());

			mockHandler.Verify(
				h => h(
					It.Is<string?>(key => true),
					It.IsAny<LoginUiEventPayload>()
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
			var mockHandler = new Mock<Action<string?,LoginUiEventPayload>>();

			var subscription = await _bus.SubscribeAsync<LoginUiEventPayload>(handler: mockHandler.Object!, key: null);

			await _bus.PublishAsync(key: "USER_LOGIN", payload: new LoginUiEventPayload());

			await _bus.PublishAsync(payload: new LoginUiEventPayload());

			mockHandler.Verify(
				h => h(
					It.Is<string?>(key => true),
					It.IsAny<LoginUiEventPayload>()
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
			var mockHandler = new Mock<Action<string?,LoginUiEventPayload>>();

			var subscription = _bus.Subscribe<LoginUiEventPayload>(handler: mockHandler.Object!, key: null);

			_bus.Publish(payload: new LoginUiEventPayload());

			_bus.Publish(payload: new InheritedLoginUiEventPayload());

			mockHandler.Verify(
				h => h(
					It.Is<string?>(key => true),
					It.IsAny<LoginUiEventPayload>()
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
			var mockHandler = new Mock<Action<string?,LoginUiEventPayload>>();

			var subscription = await _bus.SubscribeAsync<LoginUiEventPayload>(handler: mockHandler.Object!);

			await _bus.PublishAsync(payload: new LoginUiEventPayload());

			await _bus.PublishAsync(payload: new InheritedLoginUiEventPayload());

			mockHandler.Verify(
				h => h(
					It.Is<string?>(key => true),
					It.IsAny<LoginUiEventPayload>()
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

			var subscription = _bus.Subscribe<ITfEventPayload>(handler: mockHandler.Object!);

			_bus.Publish(payload: new LoginUiEventPayload());
			_bus.Publish(payload: new ClickUiEventPayload());
			_bus.Publish(payload: new SystemUiEventPayload());

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

			var subscription = await _bus.SubscribeAsync<ITfEventPayload>(handler: mockHandler.Object!);

			await _bus.PublishAsync(payload: new LoginUiEventPayload());
			await _bus.PublishAsync(payload: new ClickUiEventPayload());
			await _bus.PublishAsync(payload: new SystemUiEventPayload());

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
			ITfEventPayload? receivedArgs = new LoginUiEventPayload();

			var subscription = _bus.Subscribe<LoginUiEventPayload>((key,args) => { receivedKey = key; receivedArgs = args; } );

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
			ITfEventPayload? receivedArgs = new LoginUiEventPayload();

			var subscription = await _bus.SubscribeAsync<LoginUiEventPayload>((key, args) => { receivedKey = key; receivedArgs = args; });

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
			Func<string?, TestUiEventPayload?, Task> handler = (k,p) =>
			{
				callCount++;
				return Task.CompletedTask;
			};

			IDisposable subscription = _bus.Subscribe<TestUiEventPayload>(handler);

			Assert.Equal(1, GetSubscriptionCount());
			await _bus.PublishAsync(payload: new TestUiEventPayload());
			Assert.Equal(1, callCount);

			subscription.Dispose();

			Assert.Equal(0, GetSubscriptionCount());
			await _bus.PublishAsync(payload: new TestUiEventPayload());
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
			Func<string?,TestUiEventPayload?, Task> handler = (k,p) =>
			{
				called = true;
				return Task.CompletedTask;
			};

			using (var subscription = _bus.Subscribe<TestUiEventPayload>(handler, key))
			{
				await _bus.PublishAsync(key: key, payload: new TestUiEventPayload());
				Assert.True(called);

				called = false;
				await _bus.PublishAsync(key: "OtherKey", payload: new TestUiEventPayload());
				Assert.False(called);
			}
		}
	}


	[Fact]
	public void TfEventBusTests_Subscribe_IDisposable_RemovesSubscriptionOnDispose()
	{
		using (locker.Lock())
		{
			var mockHandler = new Mock<Action<string?,LoginUiEventPayload>>();

			var subscription = _bus.Subscribe<LoginUiEventPayload>(handler: mockHandler.Object!);

			_bus.Publish(payload: new LoginUiEventPayload());

			subscription.Dispose();

			_bus.Publish(payload: new LoginUiEventPayload());

			mockHandler.Verify(
				h => h(
					It.Is<string?>(key => true),
					It.IsAny<LoginUiEventPayload>()
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
			var mockHandler = new Mock<Action<string?,LoginUiEventPayload>>();

			IAsyncDisposable? subscription = await _bus.SubscribeAsync<LoginUiEventPayload>(handler: mockHandler.Object!);

			await _bus.PublishAsync(payload: new LoginUiEventPayload());

			await subscription.DisposeAsync();

			await _bus.PublishAsync(payload: new LoginUiEventPayload());

			mockHandler.Verify(
				h => h(
					It.Is<string?>(key => true),
					It.IsAny<LoginUiEventPayload>()
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