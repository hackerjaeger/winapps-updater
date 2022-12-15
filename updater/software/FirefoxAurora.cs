﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "109.0b2";

        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/109.0b2/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "af8c6b17eb038364d684a57ab6fbe021d6d93df7789bd92591ea68727939b7123532568fc03a994f25fcb160cde581a73c0cd43c03a1221738c9992ff39ceeb9" },
                { "af", "4beb68a8de06021c56a1dd5d4708ac56814db49b58f8abbda1bfd0480c0597dc764c80ff725cf324b3e20f0f1665c474f05b138f5e18796e979d37eee0bfe43d" },
                { "an", "e8fc8e91138b0b80afd8cafbef971f75d55ce988ee35a6f9045afdca2bc3c376f9f0f75a30a426bfbf4d8e41dd21c19c4ee638583666aff787f6fb6da143ee8f" },
                { "ar", "383367d3ddf7836f901b3ccf338aae28ad646743657b6556693d1ea9e4b214c1fba0f40460c58adccfe2e897264a4912a821859432794b5f277e33f2545c93c9" },
                { "ast", "a24429ff7794fb84609ac1697c920f11b5af17a16c68226f205cfa7861da6ef0f6f1104edfbb423a7d33056101c7b0346c7cd236681219c4d997235f15654ddd" },
                { "az", "b78212e6eeb20024751edc07a4d565e898acfed4dc589a2a5a94c1afe4ee01c25a00c0404fa51d2e768c8fdd9c43bd92445a582b0409229639e718ea74bf6b1f" },
                { "be", "602bb735bff8e84d6a6c65729abe0ebc4e6c878451169dc9d87c8e9587522c8e0dca8a05e0185b7b22e6787acc23e1f7f678342638ce420156297067ea49cb01" },
                { "bg", "10e876dff7fa1b27d5b9168422459fb0a91da9911e2a8c0eb18f9e5f2e542dfcc54cfd9c724613d15dc7f9cccf9655957a8565488d0323f6e2f9ae489cdd8d25" },
                { "bn", "fafb22386daf97b41691efb8db6aa33eaa4e0035fc05c57e1edb710f6f612013e35fd487fcab4898d5ff92193510f43ae06cab130f6f837b8605fc0a26d05067" },
                { "br", "d3a9d9f9b397fb40b601a230bc3d4ddf8c61d2d0e197219100843df974309da52e382116d0f35f0620b7b4a8f803b7c81eb03f47ebe57231d834eb755e02723f" },
                { "bs", "163ea7ff4eb48ad2b6b3475d9d385da1631728c3ad1de6a38bb16535a25a082f1d69646ce435b19f726e1cfff9d5d6572a5e532ac33107a4576d548a3d246e92" },
                { "ca", "2deb30c808fe2021b25d3c23575aab16a6fa94359830c8be6ca213187b41b6bf6bbeff7b24ddfc5c183292f9e6e5226032c63716234e79768b72dd8d03c87b85" },
                { "cak", "625ed6a3b7b1ad825c0d2224a8cb1b913be7094f0076b37f6a44545baad6d106d12679f7f3e4a4bf282af822e585495e6864a800756cdb423deb792906fda194" },
                { "cs", "d990a2916a7606e524ee8059be2b5832caef130d75b913290c365c0eabd827ccb7f19a397fda3f6931bde95e74d09c06ceae9c4d20386a1fe429fa5af8a0d44a" },
                { "cy", "6a802b824bec58d6d96c780b3dd3a90ae1f02b4d539a7dd21230e2efbf056b1bf81e3bb1ee822cf08ba914a65edbe24c20f0e6a08b71678665d9da1bec406824" },
                { "da", "7d47942814ad21b6ed27c457112ebfc93107701f06ad750b58b25f4bb81bef18fb361e8d138e9380c1106a8a9c19179ebba056eb63505d72b3b7cd36e86eaafc" },
                { "de", "ab25b058c784c1f954e25851c655895d15f7957725fe90bc745cb3a222e3747f93d17d79a0a51dbc226ab58ff8ed4ec906cd703e628d81b45b1a23f2ae2d3e61" },
                { "dsb", "0f7451cadee1d27f8f10a7f2e1f5f9a55f7cd83397e035da396bcf02c0d590a876b7f67edd446f7f1b878562bed9fbeb909066ae67a8498cc80c403ce52d82ce" },
                { "el", "a7f94cd379b1dba3290e8c5862cd9117d6a8fa148e2d402e2c9216c245885b2bcafa34ad607aedb61c51e74711490fa054f443cea295236742acd38b97ee0809" },
                { "en-CA", "cf306a5440962f59e63d965813af2dbec98e33677c48b433a375ed71900fc09f467b4d06ce2319cff2db5ffe33d77711149f9aa651f371220b388f772983d1f0" },
                { "en-GB", "955cc154ba104d0c40160062d32abc932685899fc85820a7687f100604ffce01a0e445e2fb97247378a3fdc2128051ad314c9ae242d9133bf0160df4939817fc" },
                { "en-US", "c4d5bf0488de62aafef1bbaba9d622e1902383c8657ab6402cb887b6bfb9f4b004a64b6f8a843350a1f73bbb13179f0b747250896ad445b0be936b2280d2e5f4" },
                { "eo", "819c4b2c6066da4808b7e37fe976f6a764e8a4d132615a98cabaf2aab495bdf374333eefe8d1ca87f47bca50ebcef7d817e428c50893090700ff223372c8553d" },
                { "es-AR", "dd4ce4f2cb6ee513aabf06771e66ea5a737db73d65ddbe4315618f71ec015f5d946b9e5b4304a800e3dbc5f6fb9e8aa2c81794178c432975d405016092e15910" },
                { "es-CL", "1a11bafbaad78bbb47ca036a624e9da0398fc0388a0b4b33afbe099fd184db028de160214b0d18d11fbb8d9d64fd36159f34f07f015c910524a47a351c5b33d3" },
                { "es-ES", "f24e648d8460495a2e96d6a5f5ae175ef2c74ce4e10e7be74b5ceb1822dde0c42d4c33b71b61b1c55589a92b98d309131b58e8469d37a974b52f3028944b32ff" },
                { "es-MX", "dbae4b69a67e6402e3e86f37c1551f8b92e0eb55abecfe21e3d104cb37ced31ab40b5c768a0e56576ec1552e16c04957c9ed4d50af6352b2db2d115570c1285e" },
                { "et", "5476f5fdc6f95c9841c3dce295c08c99a2c6978d7d9bd40ed15bdc35046073f58990ed6b73988c0d5b1015f0b8ced09b410d8f28fcb110da4e6a84e91e2cd4e0" },
                { "eu", "a87e1531f841090959a879c13b04d8b5042604f5ce02c4ae260a45086d666d659bcb5401c74b52264b95924a603cca8ee944984856bc5fb78ceb9905722672c9" },
                { "fa", "5c3b3431762a07c9eac68683a717a7524b99d7fa0513e3ac909aaeb2e49c06e73494240d1f5b78d035c35a143faf62a280b092bb1651b39245e1759abd9c0ab6" },
                { "ff", "8e6d1f7c8c6642dac34b7943f1b205676697f8c307c8c72eb81e330d6869288f13c27f200d550a4617b81876900e34d23289d77e7c4d232ec9e76a58c468ce9b" },
                { "fi", "072f017850d7fa473825f11a2b057b998ca70a48144108f4e7635e284527aaed22a6f47019bee000af69bc581a3586d9e40cb9f118e83e95975e319457de1042" },
                { "fr", "12f1b52fa0544c6e6c7da18b2ce1c146e125dee57b7536be341d3d1df0647a5de93fb7c9464623c902d5f0cb7cd8d8fe748d9c0a2e72b92fe51f49948cf3a0cf" },
                { "fy-NL", "4833f3c7d6b7b7c2e0f84c71d2b245013e44672e92dbd15a29136b9535a5558518aa0e1bd7c2d86ed2e734ee3297fc78e5f71892507c5f6e65de80819aa2e3ca" },
                { "ga-IE", "83db7c2b7f161f111afabb9e1cbf376be57cec48842fe7b9b285fbe9882e8033318c469a25c502615aed8ec78d9cef11677acde723fcc0a3b8ed44a9305e587a" },
                { "gd", "033f0f6a9cfbe765fdd3eaf4ff3a6f25a66259e0e09e18301a2f75885a56ccddf4bfba5774951e9b6a28cc2d063c56315d49679194dfdb7c41be68b1848ad33a" },
                { "gl", "2ed588d4155dbfea657dc28dc3d3ca00f06faff4062d20a7dc18d57657ab8bc929fcaceea9b58a83c66629925f753c85fb80fdeffa11cac3aafbe679e175963f" },
                { "gn", "458fdef30dd889dd98c67d20c929f9c33894167e2681d5fd9f58db731f74869e80fa9f1075c090209b5b13603381339d7c4f6540af3afe1b8e5dc0e8e0bce402" },
                { "gu-IN", "f0db69d00287550ce6823360a04fbb1ed5bc8f810b901e04c209cdb462e5e9b04e344b6a238150e7fe315a2a3c0036bcb8d9c8f0709efd59eddf21f5449a6f20" },
                { "he", "285d3d2eb6b4d9671759a71dec315d7401b1ccc2ecc3df5187d618082c2e55c8f8955b0f9821b39a4471b0bde9cffd05e6ff3472748bcaacf8e1e15e01be5b1d" },
                { "hi-IN", "df6c95fe599bb0ad3866e7b3b3e47c469941967f0cb9725018c3e6209b2e4505f5c76e6e3b97c82c860e57d92bf6b59c17cdc8c80bbe0e4828484205820a584a" },
                { "hr", "c13e08621b3a8fbc143877e25c1de2b32000beaf248091ecae29fc04f34a3aae3541f52521ddff1b445bd0498224e11a69e411770c19d63abb4730b6d1b73a9c" },
                { "hsb", "84cb71baf911f1575c292789b9fa5b35e70efd7b50a36a48485e6508fbdb6a92485b64569232baaf1bb66c6edf428b01c23a16deadfdb3a77a7e95bbd11a80bc" },
                { "hu", "29e54ad07dc91db65a940402328fda1c0879a185e6553d1153e24567b0fee0df8ccf0687d41421b7c5f3d9ffad2f950867d2d0a9fcfe90585dffb911d49243e8" },
                { "hy-AM", "a2160570073a5053a474fcc63621ab77876c34ae33ff07aa582d5d714812bc343304a2e5f799439a28d154b76e50f796ae439336bc7c53da6c286057d748f2f6" },
                { "ia", "c2699f5a8f911cb32aabb681cd2491501f9b8e309584e5b4431997790182ad32a61074494c31c53b6955e2592739a24c370879b187eab55680923dd174c5889b" },
                { "id", "9e0b8f04a6573df9a1b27914cc73cac49520444922df53d8c7dc46b995c68b3a0914e60fccd314e789ab0f13487655f1d6308ea9e8c077ea68440c4b9a3b6014" },
                { "is", "64b69a638f221673b05855fe089af291dab9826a521fbc8510773d260040f55b9096fcafe5d0ddd9f0f52fd998a51b80e2a32504c2663665d75076ce743e661a" },
                { "it", "8ec8d286e7c2884aa73bf9ef4e567c833ee2fe7fa85755eed69323951beb191652dfd3efb29f807642ddb55adc04c4925b931fe9c4bf36d9443ce25980f2f125" },
                { "ja", "8cecb4e9f686f48fe747b39e54924f5220d1ec97755b8dd7c0ada7d2818b05174ba479adb7d7c578e0218a932f1e1d48f3848eea21b2f977c2fe535d7aaae04b" },
                { "ka", "c7035b50f75f5751841992ff0fe0b87a8538fe27285e85809d259b82e6f1abedca76d3607a582c321404281d56f11e280c6263a6dde63e49fc1fc85610e069d9" },
                { "kab", "a74f55665cf0098559360718b32af5a6469c5dfa3adef2d25c284ab85ef582c6f673c4acff193abb60a317d05817083999c21ae4f235a12e627c6d53149e3e98" },
                { "kk", "24365a2cdc18fae812e6bf74af081a602a7dc99a418d1a7545f08a379440a57bb1c5dea96e9c0140142c9332033be0f764f13cca726eb9180efc8b0388eb27c9" },
                { "km", "3298cd0de36bbb2ca17c08823780cd8da248895e43c86d0e919c5b31e57dd4780e13a692b4096cc915d3b21f525ee1c446b81f85ccd2b3f6cecb56a377fafca9" },
                { "kn", "17c21678fbdb1c9d5801796f8c5a1c638e8ccda029be843f157f8fe9678e560510c360d5b9ae3b8ff40d8b63a9da6332cc4422558e1ccf7a57505425a25ed68e" },
                { "ko", "055de767f4adfde66387d77f6823cdc0cafc3caed474fcd1f723bc14b659796d7cb58337b7c201c9a97460e365061415d4850ab49e7b8991b67d872b785fda16" },
                { "lij", "d4044b96d69c8f0ac484d23ac6ee318aab10dd980dea63f7f137bc5136fe361d26f2ce708d877ad22e875ddc8e4fe471aeca59068d3d3a778ac185ba2583f258" },
                { "lt", "e96304c36b9d69dba7ca57a8dbe61ee24e7279063708e5bd2135d1e8909d68ca3df6a7b7e259f667b79bda3fa984371810542d1d8a1e3eb892ed9144a0f1c67d" },
                { "lv", "c3cb67017030e27f191b513decd3473b4a33dd31215d34f50ae54fe7458da0f3f9eb70fdf7a865b840a9954ffb28c6577a19782463a52c39065412a317ad274d" },
                { "mk", "afff773409268f2219b7331596e002871666c9351514f9f475a1eca7fe1ca8d7e86930d0005c296a85d019aa1346ca04adf219852f1ec4ef8ec6d6ea6ebbbd88" },
                { "mr", "ee90f450618a76332f5e064b8ef291a48661b49ad0406a8f9f5cc3fe5fa5dfe7f01456081e0dc9497359ddb2d7609ea97b98d0836c9aa25286d32e93604898bc" },
                { "ms", "87813987b68e5f4a8d3ce88e929391e6f6525a31122ebcc09d65705e3715ca2a2fa1de3ecb7278c93a6e62f10feba166c73a8a637eb4837ee48fdce09868062e" },
                { "my", "2b180918e858987db13eb65b54380840b31c079c31b836a9ab1b01335d6fd052071a32842a2fce2320e4ce0e403155c995064cf93f4f0f336eb85cab30459469" },
                { "nb-NO", "7ecce044cf3c611c1442ab4e90f064b176979ae271d8f7cdcf92224bcd71fe7522baba3ef04a213b1b18465a41e5747456c99a61893a994a9eb088c2c3a4da66" },
                { "ne-NP", "bc28c305baf9c637da81a1abe9e20d948a59e2c71d2fe39b6445c6abc88544956f7737fa0d1a3a5cc5db83e55423a58a8b55ed439bcffa3dbd9c9c998a12adcf" },
                { "nl", "4620105c2539b995450ac1cdd021e7aa9e85fdcee2b5d405b71593a679684f785511da1ba3d9be35d1ab7b4fe87df3a49404f77bb2ee3b339f76da36384cdf3d" },
                { "nn-NO", "c5db3d58c0c8bbc4863500b12b63800ed195a19d578802f4741f7a5f42daf68459b0052a179a29dcc4fa8399772caefd52941f0da22705452d57c853be3061e1" },
                { "oc", "1f46a8eab321bd002e2093c271b4efc845e4967f3bd23b15272656c05d32784ab9df8eae01a1897616db57de8693cbe0b8079934b6b810d85dc08ee8bc7135ec" },
                { "pa-IN", "5612751df90131db9ce27d6d2394ff9f75dbadca362685c779c0b8ed9d50d31535abe79f941f98f1030e5fcee700485680f92651bbd2ef64060b2f9955ee724b" },
                { "pl", "5d7459a23617ff3b16a4e167e07e997e13452d6daa57d5c6a84205b2317ec2c2ca1fcaddc9e1606577d6522b5aff7d3367d02b11bdf64cb744f089c58950dda0" },
                { "pt-BR", "883065a01ca84e1dcb5aeb15d2eeb2fd04ec423c2bb3741633673a4db2155da8dc9d71532fae3a46ce6b14c267f1b5646cf92502bfb5327b7df8720de823d312" },
                { "pt-PT", "fc4cbbb58030d9d05b020ab21377aea85e50a5ad9d53e7d0638e0a66778d13f62a29510fdc18540381e73cd304895e34375d5c5e81061752e3b42de9e5780dd6" },
                { "rm", "2edf9c85098a915561dbab9d52a4c2853b1bed1cd2330ba27eff24d325e3bde240105f78bb0618c63799a2c444ee51e3f55de106d3c6383607ef018e8ef5d96a" },
                { "ro", "146bc5c67849ea850b608ce4f9f27b9ece7bfd8bd94539bcf3442ab8fd957fd11d262de0f9727be274ee1dac4ddcdf2cd4e0b07e869e8caa13c2b2279d1272f8" },
                { "ru", "9c718a79559ff998d96fb4226d9751d26f514183cb874c5da2f5572728abfa7ea3ce46a77e917f7d44172c952d8da6cc6498d070aaed4f9eeb42cc1d4aa4c18f" },
                { "sco", "4bd605c38080f53795ec51525e21727a3b6f40c47c0a91fdf58914a9eb0e8c37da52d10442c59956e11855d6014a81c25cfe449551eac18504ab9d90c18274ae" },
                { "si", "cf976d20b59c70dd798505eb37e6db0f2adae7ea4a6efee1ab6c71d56ab1ad29ce36828ae418abce5acbe645501945a94e9d07eae4283246c4d363ef6fa48e4e" },
                { "sk", "25cbe133dd4779a2aec05fcea856f2dd0ad562e8b4ddeb1efe9eae8d686ca48ee46fd77a37dd020f7cfbd5960132f9b0bb0576baa1c9d2817eccc31084e18581" },
                { "sl", "1712a9a711e356ac763f25f14cec5280a5cfef641f14b4460970af987f468c194a087af7af7101eeb8af038f2cca96d92a6eeafb3165d2bb3696f09c8d48581d" },
                { "son", "12a75ea4952ff8911f0aec424965098bfa352b32876d6a85c89739ec229b2d5361fe07d50f61947fabab83fd287a0d33324d72d368b13228d6890a55ca60c0a1" },
                { "sq", "170ac9d66e1889fe94ea090cb8f136d02dfd2d7daba2241970f93173e148c55249b106fe7c163034ac11e56715698fee4b27296118e7d4f90777e0a174725d67" },
                { "sr", "8512f1babe3e12e4ae06e99e835db6f3f1e04292eecbecce8661f8a4ace3d79aa517804ff00c086749d0c83b63993bb5e0ff6329d4e46080cb41ddb4ca3a0a26" },
                { "sv-SE", "678daf4ca51e3c9e1c970e62261f4f68dd7acfb0c261c78798537ba95c9354bb54e04f8efd41d07d08c752faf5df4e9db5b31752efb060cee95ed53b1e47cb50" },
                { "szl", "bd7e33a73d0ff43456f2fd377aadf8905f7aee5ed3e3a43dc9c585fdf64d0f4f2f8ef392946473ab4c4aeff09e3d6c2ad3120839451faefece1549e3ff189a59" },
                { "ta", "0e7d8f2924d37519b74562e7dc84d5d00412ac43792b73bb4268c70a9ead2a79c7c2bdd69eb59457fd95aecd8f9b79cfcd877ece032584698c25e051f2e29c02" },
                { "te", "006fd71d068c8340244766257397e52bd1bea5370e35152100a7b9774363011fe562bb6c8d74ea935e92fd188aa8463ef4fcc1a8438f5c59a870b5d24fc8944b" },
                { "th", "399956aa93066815fdf7de0ed9e7f7a1dcd00d10c9fcafe381506ad3d16ff0debfb7eb1ce0a96e58593e905bd815305ab3db9fc62d5da3fa7ff5b4c2788a39a2" },
                { "tl", "ce4494617c85a783e4bfab97ff34e92fc68b3dfb417b764e0f7cea4f5addb9ff26c8e1451e26e441487a145dc0c6331ffef3559be80f769287fbadc30c222785" },
                { "tr", "b66e1bbd75cd4ed3851cb83c5c4d7009b53420042d33a4d12962a2405d83fd871f1586a6329964c96d364de72f7a26ba8f53b630248dc7d639e9ad862a7b94f8" },
                { "trs", "31f46301afe2c9bbc3077bf0be9cae7cb31062c67c238709abad897971bbdc0635397e5d088c447064034b7905570eed00f3359b16c324a91d6bf5f988dfed44" },
                { "uk", "ae63fbb817d74e8c74a66c5db9982d3a644f0cd98a2701b26ed8501101f82815a4c91c2d63dc33a5fd1d10a8a0e4ee2eccbd033fd87e29a3edc7bbeacd71cea2" },
                { "ur", "538f9e7cdfda4edec3d4311dcf979dca378e111ae4989365bbe70b4a976d762b3a651a85457f426101e7a2087b0542ed6015e65d35d37719aa72fdea6fe3401c" },
                { "uz", "e25a6363f1681fd20cbbc122ad304d5306bb99317ae81b1e63220c13aa5c30c21225a7186267cedfa7ad28aa0de2bf269ef9358b416901463dfc74d7c6788d18" },
                { "vi", "cfd082d222e3acae4bb901c0f8c5f41a568539010910686067544b4127713ac44208e65449bbb98af6444991293b448da30682eb223b730f22ad16edff16e5c9" },
                { "xh", "a29916d48a0cbc9ae855f9671177240bd21bca0a5e1503bd50df15315101bb1e207806992bf95cf849ddafa49293200342f0a7a2c263c22464bfaf4f92790fec" },
                { "zh-CN", "49401dc7cc0300367023d059c9bc4213380576afe6a697af232ee563dc14656d1ddb7eba99013eb4a3047f79d7071a27203945f571d696b76ca505fcb0bd25bf" },
                { "zh-TW", "834e6cbb439cadeb792f638b577de48d317faabe619dc1bd44af2b2f1c2c9675cff5cdda38ad0cbbbafab005d1a52edb7f558a18c441144169d91e67a63383c7" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/109.0b2/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "8b46fc17ca0ddac17f941e019a23487a7a6d0ba88321b39c9675f6b3669f38e333190021104db9d2b683fd3c5fd02e0a136269450844c6c44161d3bdeb271272" },
                { "af", "dc9b29724a4835a4499c9ee50f187d5d44d81402db555f969944baaa1e81ff72c511e8c5db06ea07eaf318f4f93a99bde5b02333c3273428ea6432093c84c274" },
                { "an", "827036c9950134a990f782b14135fdb35bdb35fcfcd61419f21fcaab454c0db70979b762a9d5d5f90d794a10ed5388769437244b4c3ae416680b82e2f2d50c13" },
                { "ar", "3d56d3f3d850864b27d6597d523061232c764d3f8f4289ec224689eb5a7b9c13e0f969ebcf178005b899bb8389c392c5df6bf879e7584221f5f069f3c873f5cd" },
                { "ast", "27c47a62de337bf8daff7ac26c9b7d1e51744143ec9072199a218e881e812df52a14df9ed05accd6d83d09b0d58c2fa1a823e400597abc12b6399b33b397f68e" },
                { "az", "30c445034cf64ab4ab02b586e2b89ff0cb95f35c68036e4152b74570d2a0c28e6c5f867bcb20f8107fbfb7d820a67083987c4733338d5097ff14bc40b586e4a9" },
                { "be", "7812ed4c65f5bf626fac64f4eca8199caf9fba5063404f7536494ed55a2927dcecf15a83bec953a40991dacf9f61d2628bc3246d00bb472530b9882ef46b49e4" },
                { "bg", "017391650095bc14de5c514057a7b9ffa2013dbe0ad70228b960ec5e236d535dfe118548e438379d9de5b093b193c830301d5ccf73b45b964d3f2962fb12e6b5" },
                { "bn", "5d3f521a00b47bb122066ce15f04217cc6a48387a1c31fa6b323c5a30283d02267683cf32de6245a1e0817b0982a413b008ddaa4572f8d15faff82c2cdf2acae" },
                { "br", "3622ecda8a9e13504c7c9350c25695cdac51592b9be39487bbbc5e3c946daed6acce79c9c284931ed3e5ab991253a7fdb3c63cd2144b3d22be5279f406a327b5" },
                { "bs", "ee66a5600c9f0feb52ac9bfaf61cf0efec45cd474f0aa9c66b1e4e9aac313fad2b96eaec0d6d3921ef846783dcd5abc74c5028e99b0328bd27356172b836e1b1" },
                { "ca", "7818235825e4f728f9544156601242d14bbd4b9e7796666063decdf3ae0c651eb568815a6d90471a4fcb59ea1b8b4c63779004e6d6c7a5d10faf3ba8f9fabdb6" },
                { "cak", "523d3c52eac5d6aa05630d03625189ddc59f203d718bc9f7f597fecc4cd37d1a6d92e73a3785f15dad8f577fbb4ce4c9016d926150f80398c1828cd684f66962" },
                { "cs", "e3536d7ed07ba36b8f816100d8adf37eb7a931a7c5cf046ab0b9ac47870bee24b4864f6cb39e908e958938d3bf7fadbf4418c5a53bcb1e4b5bf783e582b287eb" },
                { "cy", "82222039bfca777d757b33a80d60165efe94609c5144b6ef77be1b12f5af109d9e6e1633cb0d429d1401480e03509c56c5fa7b4a8f485ba2ea24c9242e8e2b94" },
                { "da", "65e55ab96c3fde18c4443d623a879e3b311dac1dfb0f2a27f7b45a974ddd00dc52c72b6d2a87ee4159d33c0a31e053854ded530b2efc23f66de4fe0b218fecb2" },
                { "de", "d832bf185fdd50d7176c69333b5980282e50555e07f356202dc205fe8114936d63145c3d47a76718dc920543ec77544c94f6fb674009a92212d591b42097b431" },
                { "dsb", "1cc9c0a740cee7bec4800a85cb065b6316e98d846f01b637fb3ba7433100c8f9076c4d0504e274d91a99b2ad1d6814cb8afd25e63b81b2ede5ab92cc7f8ecb51" },
                { "el", "e4061176e83fda863fbb4236d18184025bf00550fbcb6cc33a7712f0c751f2594a82e8e3b40649af0ba051457f590ddb09d54efcbca4c008468f2b7548622c23" },
                { "en-CA", "b057ff29a37885c9d7378b7b991a4fb025ba27461bbd3b6f7d41c75072fa696fdfb8c76d7b65aea327e27dbc0dc8297b5ddd69b4ced86ea3b1a058f2be2c015d" },
                { "en-GB", "f01e975ff09506b247b541cff1944cbfff9ad1c790475f5215b2a548b983ae72c96499b892dfb7f9e1985e3966759ff7db26f365c1fd9edd48d30a3cbaa42436" },
                { "en-US", "e659e064a9ad789b53e043be7998df6e63677999b5cad4bc85c0aa3a3dca7028988447556719a79f95665be25f232fd4f75dda36541716e256d8e1232c822957" },
                { "eo", "87d7353a2cec9a3717adbc0b5ea3a57accc2b0b93bb29fe214961687e05f3a797a413f1fe97086ddf895c96ca52c28676be0b5377ea517b3d4b4ab7c12a3ce8b" },
                { "es-AR", "ae5c4cf9d1bdf7a947091be24c983ccaa9794384534b71a80a8687e0020d34b90ac7c99b75ff25103d35641908df6b6b86beef9d00b65594501f5865eab3edad" },
                { "es-CL", "75f3871d0a02a8f797254c0b717d3942a0362489ecba715f8ca9c07cbc7e892b28a3e6a08a2949ee66c761a3fa10e9712d812c3f8b2cb5129e2c19a68529cbc5" },
                { "es-ES", "ead55548f7d46dd2833cb602b116927278330b56fe4807387b77d72aef32733cde74bf836ed08daff117959e66c4f7f43b9b7b8ac96816029830f12c5512edaa" },
                { "es-MX", "d0770c28e8c23a901f6352a4f4e2a65ad746debccacb94433f75ebacf269330b4d075085e5a194c8ee9388a09e004e4ae83fd27820770865208465bd30e6724b" },
                { "et", "72a5e55177395b416d72f15e99230ed56cb18fffe19471111be74ed8c2c838d4683c9939677773648b9fa67aa6888217cc0fbc83053c1cdea8067b9e889e6013" },
                { "eu", "272c595bdd89adbbee8d37d90b0f5197e53f08a0efed796491c5a6dba1f8130b82bd27efba6648c3424ab2be0bd681fbd415ebd4506fff854ddab27ddb4f8908" },
                { "fa", "98c9e4963f6400e3d14533ef8266f0de0bedca8cbb297243ccac36b08dda5a27da1fa25614b2fcc9a7b6b1eeedeabbc5bed3c6820ab9161530ee9c33e235171c" },
                { "ff", "28373e5152a1ec851f0ceab62e4c35ce9cb4241b4c0d2229a298c6dde749423b596da11467c0b6561db3ab66d0adf5eec4bb35f5e0c242447f2bf580b1db0b62" },
                { "fi", "793575672aa85b06f7de834e935c6fb02aba68257d884ad6efedfba3b6492658c9af6799401646be0e99390971c66460274346fcdb0a21c116da8d4835e1d4a8" },
                { "fr", "bd920a00acf29dd6a6a1b10b8bdf967c25f8d4ebc4843d260dee428d7a9c5bebd478d7b2ce782d7b082528603840dcc25f0e7ce6458a01786351840ad39d34f5" },
                { "fy-NL", "4b73169a2f3a09a6268df359d2fcd2ac8617085779c233f54fd37008c56708afb4073523531486e438ec13f18412e3a5859e71782184662d7bb00faba406ef01" },
                { "ga-IE", "a77db797966d0627bcdf8116bb2c8a76343262d29756a0801bf4b55c253c6d73caceec53868033d784ab62a4144bc302f7d5d8cce44dab9e3180350dc344ae9a" },
                { "gd", "5f7e83e0c55e8c0129a5b3868011537c7770fe655524c103ad3f5e0dc5e8f66c136965aca6dcba078565a18ee26629ae381631196de239e66c4c8065a7153295" },
                { "gl", "1b5db075bad271206dbfca8739584282562c47c0fa471d96586b2c5b04a23af9f39874b98a863012942ad5b883da53486b87d6ac24909d1a069bf44612fb9c09" },
                { "gn", "cbaa84b102e73aa88bcf812122678acef7ef0a7ba79d574eeb12ea15c544a343f2bd66b1fed47b594ce0b64f3a5a5db5991bac1e143fe6df8175beba2b4dee4b" },
                { "gu-IN", "cc6611cb7fe0c10c991ceff6fd439ae3237038fd956bcabe331bef81d2234780bb3db1e978168311ed26ec17aaf9fbdddba67b14eb18cc3e6e1375d7cf497e8f" },
                { "he", "9841a453991a381fc5793860ef258aa9b73c829af6489101e844d97a285f9b0148ede302b61de94344fabd50a7d80da75647a6312dc1620420dda354b5b89bc5" },
                { "hi-IN", "a40763c00be3bc42f15fc350b34da3ccaf257b0e3731ea07bec38694ef5adbc3a6be9598361cb81223eb74adc18f3d61a4b94d65f42b096a4a7704259e661d1d" },
                { "hr", "3b2cb363f2ec9a6ebad1738bafd13826dd56a8004a0cf01e949a8c12e1de1f374c9600342d58942c230a5aef07799f33c2b7aa7281fe8482af4d0252011ae7bb" },
                { "hsb", "8058275496ff28ba5b119409f6fd45289b07ace7e887d55d30ecab16485dde599e6c9793a856b77752a3ee8fc55dbbe366dc287b2d6c62a41004c7f6296172f9" },
                { "hu", "d5231cba622464838014ac7659eb9ab00b7387ed1460fe29cc511064ecc88ccb3d4ab3f79dd4e5a4b0117d0b2505d263f9e277a8a1c62dbc894aff9ab3734e26" },
                { "hy-AM", "74384ce204655b71908ebff80ed869b37c27d1989a33213781d79ae84475481967ca58549848c3babd548b92456976f4a609e5ef9d3cea30b1473ed47f1a1d72" },
                { "ia", "f239d66e0d1caa38361049449d99b0ef54baccaaa92452c089ea5ffa93322c88a198f65d29a4d58d86599b60475b6433eabceb41e37845f8663b4649dc5a5112" },
                { "id", "1457c85c10d2aabe4ab3ce9f0ccdb12f0f4d1ebe7bdb6996fee364688710dd6205a270992dd9fc934710d1ebd17a5e57786c47e4fcba26560d60ffc26eeacbd6" },
                { "is", "d1273ee3b9fc2dfbee6a25506bd1ef828a90170c1a3bbb45ee56d4beb64aefde0bed67e3404e2a3279243e8fb4ffe1d1384736a9ecceaf46dcbc8f788753d84c" },
                { "it", "d9ac0771b472faef5d792b906d248b79af54b99f88c1cd72cc57877b0bfc7e595e351195e35deca71d2b05e205101d7b719ed6ff3d1f751727d0a395c669f823" },
                { "ja", "35650334622d9e2f9c4e826be550b8796d733c58cdd8bad348e240eb24317d4de282a4d70c510cd6af539bdd5904b95e93cf709d85c1af8e692a8e357a025053" },
                { "ka", "5fe8606f4393beccd2a3a62dae93be5420eeabe2d279224f94fdb54cdd42e79a633170d75a107aadf0aea007f45ccdf00670de68be6141a4b877ce37152d600e" },
                { "kab", "91899c1b20c81f52f2c55ba6f44d3bfa2110ba771bb8c0e6ab0d43c300ee7fc8b99305aca05cfa07969caa88f5113c2b14d11edf2973da9f415376be56033dbd" },
                { "kk", "6948478a370cf3e6844bb0033447553e1267e491f4fc2da91a989e2e7d89a347296f2679cae36f8ac6652e81f34830a38e7098068a94c3a3c7250f89f17bda8e" },
                { "km", "477c8e0357477adb6346a06e1b8d2b380be9f49f3913b9dfafe69305b5a31af6cee34fecb5b108fff9ec3e616e3123d7dcb6aa0921e9a70a6fea1debc41f53f1" },
                { "kn", "962c21529def8c80412b1b06cdeab1ea53c4bbf500b8dce07255dc2ac34a70e410b7ab78db235aedd43d2a26a5880587e1c3cb1cae76992214403fe070cb03a5" },
                { "ko", "8cebb26e101c99cedade012b62d1bbd52e3e2275b01a51c848f6998e99f9638510af83084d922a10594ea2ae9486634500889be94e3ac7f6d4a40c804f8ac037" },
                { "lij", "f0de6d710bd925b0d13bff29f612b001b781f4872424700a678deafb0ea0b1b8c1a6bc2eec05f12c2406f2b22f49fa4b0594922da46151f99133376293527653" },
                { "lt", "d3b2dbc5aea89b3ab50bcb5d4b1933720dc3f853eb7729308df8d071f0c6d49885546d54113be276e15d06a6327b087723932ca8000501400fa6df04dbd35999" },
                { "lv", "0e897c6b45d63e07524a0f43a7d7a4a97a1abcf043bc1b641d9dca999fc1ee6b3cff376961431333d6bf02cd56805a1bf7ad41e94f687d4bbfcae35b1aa2d514" },
                { "mk", "b80a3facee98490ae763277b05dd541fc896872fd1714f7ceff68722517da1cd3ae978481a612a90e69b3e2f5eef21867643a46084f3072c0d56c03f7e7d994b" },
                { "mr", "0f48e96f0b9e84ea18d4a7e52ee36d833b6cc346beadfe91e1142d73a3b4c8075c9fa136c2921367911436957fb906dd85ea57018a4c13323b77ba4ea83f3603" },
                { "ms", "5f198c73e3c52b00d5128651c8efecbdae65efb63aea8159f41d76dd5986d76310b7a4510974e3f4af20f6ef20c252d0321cf81d020d53a4f5ef02cc4941e587" },
                { "my", "5dfa020aa5b518475a292574992b3f61478184d2974854bedb5189b31a46cca7f86d286b73ba47195e8d6f815ca08805b273bb14565b6ea8a01787a9386bf7b7" },
                { "nb-NO", "785bf126541a9bfbceeb77ef611948899e457a748f710ec1d23b9500d21d92eb477d8a5df85b424b660d1f099bbb0538b6c8091f133606d667faee6d486c469b" },
                { "ne-NP", "999eb65ea32d077eb9c384d83880d94e3dc40feb64c3095b98287bcfef7b19ebfdfac3c327aac76bb2321ac4f842fee2bd2f4ee916743688d1be7aef2f44a76e" },
                { "nl", "f94b9bc69e7d6e72eac7765926d459c98cd8be5d00cf443a62ef5d0eafbff0c71b30385ee2db07d79126b825816394a4871aa238a41a89793362416415e5ce37" },
                { "nn-NO", "2eb0260ec8071d077b3995b3230fff0120317e65ebd4cbbd64779f909f202cfce0e1abdb9b0ad3a9554f3b0173cf637d98689ee1878d248b215af5a4549416b5" },
                { "oc", "b5fb2905abce8035c9d64fcbc863b1ee5afe09891d35ff3096e848ff098492cc048ade7a84dd0e9ce40008b2ca2d18d03d412e56ab36e0009a6764a033b72b54" },
                { "pa-IN", "03b6d26d5862dc9a5e8e8fd427f96fee05664db47c51f53ccfa1ec87ac5c1f29e5d1ddd90d232668e18478f9e29aa7e0079af2b7cf8c8a9204ce4553ab6d0468" },
                { "pl", "0ce707c1db6ffea7e2d1b3bc15ed0b689ee1c6282cfab0ae92506b3bc30cf2ced39a4d7119e88cd0b1be317da9a4c956ca899f4e4dbe8149f3c4c991796a8705" },
                { "pt-BR", "76e5026812954213b93b4ec1bde01596a8a31adb72fb55d1c6dee95853b08132d0506277c0e15e906a6efc299e488c73747dc550b14dba7f4a2e1eb6e5763e05" },
                { "pt-PT", "c99bc82a49084017adc92b453f0ebe08e17b1abe777996e2ed10396b1de84c87ed4202109a8b53dd506eb9d63e70786f73ec93665e131a1f71a8f46b3d2a2aea" },
                { "rm", "28c954a3fba4e0f69ade5b0d1dfde8c6c39750e532e0ccfd5502e77d42867a92cacade5de4f6b9508091af3b8d457233d244b69308c443674de91afb8364037c" },
                { "ro", "5d67179f54f305542089eb5d8fcb986ce823729a92765774f14ecfc1a6c4c4b74982317325b7aede27c14d6934d30346c500a62a3363834d2fa1c56852217b59" },
                { "ru", "189af35bea1dc109f048ef22fca4351c60a7c98950ee975c0a2961ffc1b340007f7fdd363a9477664ea79a319e2e37f583bf5016c674e168c4b1e264e8215d65" },
                { "sco", "c5cbcf1713840a3f320c76765c11c39787059559bbd56fa91e80029ea014ab4b54837fd5bc0c27ae0b78013e19ade382bc447a4661b53d2c29c887db1289c5a4" },
                { "si", "a074a2ad624b24575013c375a8543d67e241f5a37ec4c8497bf4dc66853fb157f3a92ea2221317169fc1087be3ab2331eb6d20d9ffaccd4048c734b73736d43c" },
                { "sk", "049ff0885fe2554fab99ccd1a596057119de9db1d69c758c35305b829ffa049a87cf5c2d70171ffe2409d83ed7b626a782a8312edee4285027be3576df026654" },
                { "sl", "c81acca8ff2d2625cbef8afc3f1cdc6d41e0c09bc0efb467540d79e26e3cd7328d5b36a36d48ca3061eaa80048144315641c0d38c1d04a0cfd3909d70c11c455" },
                { "son", "9468e2f16e197b5795a42c9482fe2354ef26844a766e9af0657b4e01c3e01acf4effd69c0c405f114842420bd3e889070c9fa56ea80839eef5ac04369cb19a0f" },
                { "sq", "ea1a8197c711cfe936a92f1c0f8c63e43685c313c3fa5233405d14b8600d0f2049e89299515d1c8a36ab44e58fe9904328c115694a04a8be9e6a399a92acd049" },
                { "sr", "18d4a1b51a619d8529f34104d10bf9c7ddebc258f59aed0a59877cba23ef3a155601d02f6344b69cb91f615b58cf9587eb7f2f14fe6800df280c2a2776785917" },
                { "sv-SE", "0924aa9a593e28850ad2d5dd49e2486fc1a10b58e46bcd7f1e20c167af730c4f2d1b2cc68399ee48ed37463010da92ba2364be32dcb614ac0dd3ad293af1ae14" },
                { "szl", "c1c348d2f77d9f9dddbe534d483572b4d31ea9c9fea7f9e23674b49cd34ef0c806ef0e8fa4ea81edfea5e240805b829c52a9cad5a1b77e45497f8a37ed821cbc" },
                { "ta", "16bebb2d78620fe03bdf43444e04ccdd99065fe48879efa5f90a129052261834f7b9c53ee42157300ffbae0fbfdfa6ad2e3dc281545fc1ae2b53da5d0e6357f7" },
                { "te", "3d69c25282dcf9cc9e80888364beacbf91a8f04a6090386cb02bb20253a448548120816bef8694242e268c28470df790742c0013fc2fa379e93e8728a30ff2a0" },
                { "th", "6ed122618089043f305023a1438ec6e946b73554b6dea5367bd95d11ce7ebe522945547e0c37c3008901810ad1cc4d00330948c374bc43cb502763c6b632e2b1" },
                { "tl", "6ab621ab091a4953b429e89618ef1e0ee560b062387153d37c96fd0ddc41cc76ca9d3644fc156c11c57abcf28daf32423094a81fb9a1adea03f6dbdabfd5a3b6" },
                { "tr", "9d6c9bd069914189129a5cea350645a5bf9714f00cfe087a56ce5e8e68f8fafd7a60eebe67b0c294b858f168cc1544204e2c06e0f433245a51352778f901039e" },
                { "trs", "0d2762fc0d2431a075800b0a28de1064e2f1fd8f89cd79640a50bad33a3554ff7a08c467afd7c6ae2f268518ec8c306a7a1ceacc57fad6e096c34e41bfbbc884" },
                { "uk", "aa920e4ba4cdd1d27629f19c16e07ec50d186b0cd4b5409bb978373e444477be7ed137e98d8a0c9269d6f826fedb8b62635ff065665b8bb10de51cfcece56cfc" },
                { "ur", "0883e9f78361651e92accbc8a79b2ae75dbe2a281c3074172e27e38822d460fbfd7f5a25e7fcda7bb6bbf598aeb20750d7292f1faad9f6a531c8bf1bf08b19de" },
                { "uz", "06d6929059858632c82feeb95c56cccd9a1047133192ae65009176825f5358089f4b4f6b8dd33273fb7e4d6145a6a9a4f5072d2af5f590403cc24681662c202c" },
                { "vi", "b3c2faf8f934ff13b3b4d96b8c764eff26d69adfbe238521246ebd432a3663a79e534040e7d5fbc5c2a734dfe7ccb985e32ef6289056bf850c0f4ce23e5ea57f" },
                { "xh", "70de153ac444ca9f5829150aebb9f8036a262c9b69c2d8513a9e4665e909a347e1289907aecaca625208fc3bc70493061f3887a3446cceb3b93d13dd7350d004" },
                { "zh-CN", "1cb2ccd05519c5ffd07b4dbd20fca9d62ce588f38ef4f7e5b74afe076a95b6587e1e51908480842a98faa0f2a0cfcfdd487742755679fa53c917ab0a9a32ee71" },
                { "zh-TW", "8de48d95f0e8417708360f3412a51a7c5682399ae39075a15005b881a8e747e83fcdb02969e4943144808f031a830aa7a3f5dacae3ef0dfb651218cbdee9b246" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[versions.Count - 1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
                    if (newerVersion == currentVersion)
                    {
                        checksumsText = sha512SumsContent;
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer"
                        + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                    return null;
                }
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // fallback to known information
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
