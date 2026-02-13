import { router } from 'expo-router';
import { Pressable, StyleSheet, Text, View } from 'react-native';

export default function HomeScreen() {
  return (
    <View style={styles.container}>
      {/* Title */}
      <Text style={styles.title}>Welcome rayyan the ugly</Text>

      {/* Subtitle */}
      <Text style={styles.subtitle}>
        Control your Unity Plane from your phone
      </Text>

      {/* Start Button */}
      <Pressable
        style={styles.button}
        onPress={() => router.push('/controller')}
      >
        <Text style={styles.buttonText}>Start Controlling</Text>
      </Pressable>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#0f172a', // Dark gaming background
    justifyContent: 'center',
    alignItems: 'center',
    padding: 20,
  },
  title: {
    fontSize: 36,
    fontWeight: '900',
    color: '#38bdf8', // Bright accent color
    marginBottom: 16,
    textAlign: 'center',
    textShadowColor: '#00000080',
    textShadowOffset: { width: 2, height: 2 },
    textShadowRadius: 6,
    fontFamily: 'Courier New', // You can replace this with a custom font
  },
  subtitle: {
    fontSize: 18,
    color: '#94a3b8', // Light gray
    marginBottom: 50,
    textAlign: 'center',
  },
  button: {
    backgroundColor: '#38bdf8',
    paddingVertical: 18,
    paddingHorizontal: 40,
    borderRadius: 16,
    elevation: 5,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.3,
    shadowRadius: 5,
  },
  buttonText: {
    color: '#0f172a',
    fontSize: 20,
    fontWeight: '700',
    textAlign: 'center',
    letterSpacing: 0.5,
  },
});
