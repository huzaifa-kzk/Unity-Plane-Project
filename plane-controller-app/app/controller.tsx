import { useFonts } from 'expo-font';
import { LinearGradient } from 'expo-linear-gradient';
import React, { useEffect, useRef, useState } from 'react';
import { Animated, Dimensions, PanResponder, StyleSheet, Text, View } from 'react-native';

// Font import
import KTFRoadbrushFont from '../assets/fonts/KTF-Roadbrush.ttf';

const { width, height } = Dimensions.get('window');

export default function ControllerScreen() {
  const [fontsLoaded] = useFonts({
    KTFRoadbrush: KTFRoadbrushFont,
  });

  const pan = useRef(new Animated.ValueXY()).current;
  const ws = useRef<WebSocket | null>(null);

  const connectWebSocket = () => {
    ws.current = new WebSocket('server IP');

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

  if (!fontsLoaded) {
    return <View style={{ flex: 1, backgroundColor: '#1e3c72' }} />;
  }

  return (
    <LinearGradient
      colors={['#1e3c72', '#2a5298']}
      start={{ x: 0, y: 0 }}
      end={{ x: 1, y: 1 }}
      style={styles.container}
    >
      <Text style={[styles.title, { fontFamily: 'KTFRoadbrush' }]}>
        UNITY PLANE CONTROLLER
      </Text>

      <View style={styles.joystickOuter}>
        <Animated.View
          style={[
            styles.joystickInner,
            { transform: [{ translateX: pan.x }, { translateY: pan.y }] },
          ]}
          {...panResponder.panHandlers}
        />
      </View>

      <Text style={styles.instructions}>
        Move the joystick to control the plane
      </Text>
    </LinearGradient>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    width,
    height,
    justifyContent: 'center',
    alignItems: 'center',
    padding: 20,
  },
  title: {
    fontSize: 32,
    color: '#fff',
    textAlign: 'center',
    textShadowColor: '#00000080',
    textShadowOffset: { width: 2, height: 2 },
    textShadowRadius: 6,
    marginBottom: 40,
  },
  joystickOuter: {
    width: 160,
    height: 160,
    borderRadius: 80,
    backgroundColor: '#ffffff30',
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: 30,
  },
  joystickInner: {
    width: 80,
    height: 80,
    borderRadius: 40,
    backgroundColor: '#38bdf8',
  },
  instructions: {
    color: '#e0e0e0',
    fontSize: 16,
    textAlign: 'center',
  },
});
