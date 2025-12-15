using WebVella.Tefter.MessagingEx;

namespace WebVella.Tefter.Tests.Services;

public partial class TfEventBusTests : BaseTest
{
	public class LoginEvent : ITfEventPayload { public string Username { get; set; } }
	public class ClickEvent : ITfEventPayload { public int X { get; set; } }
	public class SystemEvent : ITfEventPayload { public DateTime Timestamp { get; set; } }
	public class InheritedLoginEvent : LoginEvent { public string DeviceId { get; set; } }
	public class InvalidPayload { } // Does NOT implement ITfEventPayload

	private readonly ITfEventBus bus;

	public TfEventBusTests() : base()
	{
		Assert.NotNull(ServiceProvider);

		bus = ServiceProvider.GetService<ITfEventBus>();

		Assert.NotNull(bus);
	}

	[Fact]
	public void TfEventHubTests_Subscribe_ThrowsException_WhenTypeDoesNotImplementITfEventPayload()
	{
		using (locker.Lock())
		{
			Assert.Throws<ArgumentException>(() =>
			{
				bus.Subscribe<InvalidPayload>(_ => { });
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
				await bus.SubscribeAsync<InvalidPayload>(_ => { });
			});
		}
	}

	[Fact]
	public void TfEventBusTests_Publish_KeyedSubscription_OnlyReceivesMatchingKey()
	{
		using (locker.Lock())
		{
			var mockHandler = new Mock<Action<LoginEvent>>();

			bus.Subscribe<LoginEvent>(mockHandler.Object, "USER_LOGIN");

			bus.Publish("USER_LOGIN", payload: new LoginEvent());

			bus.Publish("OTHER_KEY", payload: new LoginEvent());

			bus.Publish(payload: new LoginEvent());

			mockHandler.Verify(h => h(It.IsAny<LoginEvent>()), Times.Once);
		}
	}

	[Fact]
	public void TfEventBusTests_Publish_GlobalSubscription_ReceivesAllKeys()
	{
		using (locker.Lock())
		{
			var mockHandler = new Mock<Action<LoginEvent>>();

			bus.Subscribe<LoginEvent>(mockHandler.Object, null);

			bus.Publish("USER_LOGIN", payload: new LoginEvent());

			bus.Publish(payload: new LoginEvent());

			mockHandler.Verify(h => h(It.IsAny<LoginEvent>()), Times.Exactly(2));
		}
	}

	[Fact]
	public async Task TfEventBusTests_Publish_GlobalSubscription_ReceivesAllKeysAsync()
	{
		using (await locker.LockAsync())
		{
			var mockHandler = new Mock<Action<LoginEvent>>();

			await bus.SubscribeAsync<LoginEvent>(mockHandler.Object, null);

			await bus.PublishAsync("USER_LOGIN", payload: new LoginEvent());

			await bus.PublishAsync(payload: new LoginEvent());

			mockHandler.Verify(h => h(It.IsAny<LoginEvent>()), Times.Exactly(2));
		}
	}


	[Fact]
	public void TfEventBusTests_Publish_SpecificTypeSubscription_ReceivesInheritedEvents()
	{
		using (locker.Lock())
		{
			var mockHandler = new Mock<Action<LoginEvent>>();

			bus.Subscribe<LoginEvent>(mockHandler.Object, null);

			bus.Publish(payload: new LoginEvent());

			bus.Publish(payload: new InheritedLoginEvent());

			mockHandler.Verify(h => h(It.IsAny<LoginEvent>()), Times.Exactly(2));
		}
	}

	[Fact]
	public async Task TfEventBusTests_Publish_SpecificTypeSubscription_ReceivesInheritedEventsASync()
	{
		using (await locker.LockAsync())
		{
			var mockHandler = new Mock<Action<LoginEvent>>();

			await bus.SubscribeAsync<LoginEvent>(handler: mockHandler.Object);

			await bus.PublishAsync(payload: new LoginEvent());

			await bus.PublishAsync(payload: new InheritedLoginEvent());

			mockHandler.Verify(h => h(It.IsAny<LoginEvent>()), Times.Exactly(2));
		}
	}

	[Fact]
	public void TfEventBusTests_Publish_InterfaceSubscription_ReceivesAllPayloads()
	{
		using (locker.Lock())
		{
			var mockHandler = new Mock<Action<ITfEventPayload>>();

			bus.Subscribe<ITfEventPayload>(handler: mockHandler.Object);

			bus.Publish(payload: new LoginEvent());
			bus.Publish(payload: new ClickEvent());
			bus.Publish(payload: new SystemEvent());

			mockHandler.Verify(h => h(It.IsAny<ITfEventPayload>()), Times.Exactly(3));
		}
	}

	[Fact]
	public async Task TfEventBusTests_Publish_InterfaceSubscription_ReceivesAllPayloadsAsync()
	{
		using (await locker.LockAsync())
		{
			var mockHandler = new Mock<Action<ITfEventPayload>>();

			await bus.SubscribeAsync<ITfEventPayload>(handler: mockHandler.Object);

			await bus.PublishAsync(payload: new LoginEvent());
			await bus.PublishAsync(payload: new ClickEvent());
			await bus.PublishAsync(payload: new SystemEvent());

			mockHandler.Verify(h => h(It.IsAny<ITfEventPayload>()), Times.Exactly(3));
		}
	}

	[Fact]
	public void TfEventBusTests_Publish_NullPayload_CallsHandlerWithNull()
	{
		using (locker.Lock())
		{
			ITfEventPayload? receivedPayload = new LoginEvent();

			bus.Subscribe<LoginEvent>(p => receivedPayload = p);

			bus.Publish();

			Assert.Null(receivedPayload);
		}
	}

	[Fact]
	public async Task TfEventBusTests_Publish_NullPayload_CallsHandlerWithNullAsync()
	{
		using (await locker.LockAsync())
		{
			ITfEventPayload? receivedPayload = new LoginEvent();

			await bus.SubscribeAsync<LoginEvent>(p => receivedPayload = p);

			await bus.PublishAsync();

			Assert.Null(receivedPayload);
		}
	}


	[Fact]
	public void TfEventBusTests_Subscribe_IDisposable_RemovesSubscriptionOnDispose()
	{
		using (locker.Lock())
		{
			var mockHandler = new Mock<Action<LoginEvent>>();

			var subscription = bus.Subscribe<LoginEvent>(handler: mockHandler.Object);

			bus.Publish(payload: new LoginEvent());

			subscription.Dispose();

			bus.Publish(payload: new LoginEvent());

			mockHandler.Verify(h => h(It.IsAny<LoginEvent>()), Times.Once);
		}
	}

	[Fact]
	public async Task TfEventHubTests_SubscribeAsync_IAsyncDisposable_RemovesSubscriptionOnDisposeAsync()
	{
		using (await locker.LockAsync())
		{
			var mockHandler = new Mock<Action<LoginEvent>>();

			IAsyncDisposable? subscription = await bus.SubscribeAsync<LoginEvent>(handler: mockHandler.Object);

			await bus.PublishAsync(payload: new LoginEvent());

			await subscription.DisposeAsync();

			await bus.PublishAsync(payload: new LoginEvent());

			mockHandler.Verify(h => h(It.IsAny<LoginEvent>()), Times.Once);
		}
	}
}