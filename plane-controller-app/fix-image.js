const fs = require('fs');
const path = require('path');
const { execSync } = require('child_process');

console.log('🔍 Searching for problematic image files...');

const imageExtensions = ['.png', '.jpg', '.jpeg', '.gif', '.webp', '.bmp', '.svg'];
let problematicImages = [];

function checkImage(filePath) {
  try {
    // Check if file exists
    if (!fs.existsSync(filePath)) {
      console.log(`❌ Missing: ${filePath}`);
      return filePath;
    }

    // Check file size
    const stats = fs.statSync(filePath);
    if (stats.size === 0) {
      console.log(`❌ Empty file (0 bytes): ${filePath}`);
      return filePath;
    }

    // Try to read the file
    const buffer = fs.readFileSync(filePath);
    if (!buffer || buffer.length === 0) {
      console.log(`❌ Cannot read buffer: ${filePath}`);
      return filePath;
    }

    // Check first few bytes for image signature
    const signatures = {
      png: [0x89, 0x50, 0x4E, 0x47],
      jpg: [0xFF, 0xD8, 0xFF],
      gif: [0x47, 0x49, 0x46, 0x38],
    };

    const ext = path.extname(filePath).toLowerCase();
    const valid = Object.entries(signatures).some(([type, sig]) => {
      if (ext.includes(type)) {
        return sig.every((byte, i) => buffer[i] === byte);
      }
      return false;
    });

    if (!valid && !ext.includes('.svg')) {
      console.log(`⚠️  Suspicious: ${filePath} (wrong signature for extension)`);
      return filePath;
    }

    console.log(`✅ OK: ${filePath}`);
    return null;
  } catch (error) {
    console.log(`❌ Error checking ${filePath}: ${error.message}`);
    return filePath;
  }
}

function walkDir(dir) {
  const items = fs.readdirSync(dir);
  
  items.forEach(item => {
    const fullPath = path.join(dir, item);
    
    try {
      const stat = fs.statSync(fullPath);
      
      if (stat.isDirectory()) {
        walkDir(fullPath);
      } else if (imageExtensions.includes(path.extname(item).toLowerCase())) {
        const problematic = checkImage(fullPath);
        if (problematic) {
          problematicImages.push(problematic);
        }
      }
    } catch (error) {
      console.log(`⚠️  Cannot access ${fullPath}: ${error.message}`);
    }
  });
}

// Start from current directory
walkDir('.');

console.log('\n📊 Summary:');
console.log(`Found ${problematicImages.length} problematic image(s)`);

if (problematicImages.length > 0) {
  console.log('\n❌ Problematic images:');
  problematicImages.forEach(img => console.log(`  - ${img}`));
  
  console.log('\n💡 Suggested fixes:');
  console.log('1. Replace the images with valid ones');
  console.log('2. Remove the images if not needed');
  console.log('3. Convert using: convert image.jpg image.png');
  
  // Create a backup and replace with placeholder
  problematicImages.forEach(img => {
    const backupPath = `${img}.backup`;
    fs.copyFileSync(img, backupPath);
    console.log(`   Created backup: ${backupPath}`);
    
    // Create a tiny placeholder image (1x1 transparent PNG)
    const placeholder = Buffer.from([
      0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, // PNG signature
      0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52, // IHDR chunk
      0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, // 1x1 image
      0x08, 0x06, 0x00, 0x00, 0x00, 0x1F, 0x15, 0xC4,
      0x89, 0x00, 0x00, 0x00, 0x0A, 0x49, 0x44, 0x41,
      0x54, 0x78, 0x9C, 0x63, 0x00, 0x01, 0x00, 0x00,
      0x05, 0x00, 0x01, 0x0D, 0x0A, 0x2D, 0xB4, 0x00,
      0x00, 0x00, 0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE,
      0x42, 0x60, 0x82 // IEND chunk
    ]);
    
    fs.writeFileSync(img, placeholder);
    console.log(`   Replaced with placeholder: ${img}`);
  });
  
  process.exit(1);
} else {
  console.log('✅ All images appear to be valid!');
  process.exit(0);
}