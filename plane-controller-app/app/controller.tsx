// controller.tsx
import { Asset } from 'expo-asset';
import { useFonts } from 'expo-font';
import { LinearGradient } from 'expo-linear-gradient';
import React, { useEffect, useRef, useState } from 'react';
import { Animated, Dimensions, ImageBackground, PanResponder, StyleSheet, Text, View } from 'react-native';

const { width, height } = Dimensions.get('window');

function usePreloadImages(images: number[]) {
  const [loaded, setLoaded] = useState(false);
  useEffect(() => {
    async function load() {
      await Promise.all(images.map((img: number) => Asset.fromModule(img).downloadAsync()));
      setLoaded(true);
    }
    load();
  }, []);
  return loaded;
}

export default function ControllerScreen() {
  const [fontsLoaded] = useFonts({
    KTFRoadbrush: require('../assets/fonts/KTF-Roadbrush.ttf'),
  });

  const imagesLoaded = usePreloadImages([require('../assets/images/airplane.png')]);
  const pan = useRef(new Animated.ValueXY()).current;

  const ws = useRef<WebSocket | null>(null);

  // Function to connect WebSocket with logging and reconnect
  const connectWebSocket = () => {
    ws.current = new WebSocket('server IP'); // Public IP

    ws.current.onopen = () => console.log('✅ Connected to WebSocket server');
    ws.current.onmessage = (msg) => console.log('⬅️ Server says:', msg.data);
    ws.current.onerror = (e) => console.log('⚠️ WebSocket error', e);
    ws.current.onclose = () => {
      console.log('❌ WebSocket closed, reconnecting in 2s...');
      setTimeout(connectWebSocket, 2000);
    };
  };

  useEffect(() => {
    connectWebSocket();
    return () => ws.current?.close();
  }, []);

  const panResponder = useRef(
    PanResponder.create({
      onStartShouldSetPanResponder: () => true,
      onMoveShouldSetPanResponder: () => true,
      onPanResponderMove: (_evt, gestureState) => {
        pan.setValue({ x: gestureState.dx, y: gestureState.dy });

        // Send data only if WebSocket is open
        if (ws.current && ws.current.readyState === WebSocket.OPEN) {
          ws.current.send(JSON.stringify({ dx: gestureState.dx, dy: gestureState.dy }));
          console.log('➡️ Sent:', { dx: gestureState.dx, dy: gestureState.dy });
        }
      },
      onPanResponderRelease: () => {
        Animated.spring(pan, { toValue: { x: 0, y: 0 }, useNativeDriver: false }).start();
      },
    })
  ).current;

  if (!fontsLoaded || !imagesLoaded) {
    return <View style={{ flex: 1, backgroundColor: '#1e3c72' }} />;
  }

  return (
    <ImageBackground source={require('../assets/images/airplane.png')} style={styles.background} resizeMode="cover">
      <LinearGradient colors={['#1e3c72aa', '#2a5298aa']} start={{ x: 0, y: 0 }} end={{ x: 1, y: 1 }} style={styles.container}>
        <Text style={[styles.title, { fontFamily: 'KTFRoadbrush' }]}>UNITY PLANE CONTROLLER</Text>

        <View style={styles.joystickOuter}>
          <Animated.View
            style={[styles.joystickInner, { transform: [{ translateX: pan.x }, { translateY: pan.y }] }]}
            {...panResponder.panHandlers}
          />
        </View>

        <Text style={styles.instructions}>Move the joystick to control the plane</Text>
      </LinearGradient>
    </ImageBackground>
  );
}

const styles = StyleSheet.create({
  background: { flex: 1, width, height },
  container: { flex: 1, justifyContent: 'center', alignItems: 'center', padding: 20 },
  title: { fontSize: 32, color: '#fff', textAlign: 'center', textShadowColor: '#00000080', textShadowOffset: { width: 2, height: 2 }, textShadowRadius: 6, marginBottom: 40 },
  joystickOuter: { width: 160, height: 160, borderRadius: 80, backgroundColor: '#ffffff30', justifyContent: 'center', alignItems: 'center', shadowColor: '#000', shadowOffset: { width: 0, height: 6 }, shadowOpacity: 0.4, shadowRadius: 8, elevation: 10, marginBottom: 30 },
  joystickInner: { width: 80, height: 80, borderRadius: 40, backgroundColor: '#38bdf8', justifyContent: 'center', alignItems: 'center', shadowColor: '#000', shadowOffset: { width: 0, height: 4 }, shadowOpacity: 0.5, shadowRadius: 6, elevation: 8 },
  instructions: { color: '#e0e0e0', fontSize: 16, textAlign: 'center' },
});
