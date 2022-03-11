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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);

        
        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/91.7.0/SHA512SUMS
            return new Dictionary<string, string>(65)
            {
                { "af", "c4a57f460a6064d3257c0e873e546cf94617fca19bafafbf994a4e75bd7b2b6d914d0a2399589d19ccf0c6695b0eeb40387ef223f2fe7492c0a008b347a80f94" },
                { "ar", "806d4d6eaa01aeb212bb1f6d3032216eb06c76166963fda532e715f30dadcecc43a18c357b0d29589d7ba06dd79273bcfdc871a4df057125d4b82adfa05dc5d7" },
                { "ast", "bad1a801e51e96650fd29b9a9bd8129002ad0458f4ecb11171341705e6cf9a87467a46fbcfe062c5dae1ccd971dc810f286b612ce506f3693d6d3b1a55e4f7d0" },
                { "be", "7e708ef7d3cf52ea28ebccde83d75c3bb0e3f4820bac370739dc09a55545bb27338dc3f286604cf8b12c9bcbff9a51038beaec746342a7104bf474986948b2d4" },
                { "bg", "275966dfd74a1dd0d15fe7658347748130650fe730c0fc0a28e2ecc237067101967669c1786a9f3ebc17ddeda5142f3806d7121ef68d280d26b091184980c885" },
                { "br", "c21009e9fc7316585df7d377ef198fbcdca1035a358fbf5b9555e459b0c20659eaf2e1a6fffd0dcb1a9a02b9cbed9488892c294a8fbd806139891a9f756d7e88" },
                { "ca", "181e4eec0feeabb6c2f60cc915d7a6ec60e2af4c6bccaaf343f0e2340a8206c41c69b60027b851c3d071dcef3f46079ceb0f543d6969a08458f517cd9325562d" },
                { "cak", "9c8cbbc65a9dbc577c3663dbf3866c4a1ed1fd738518eedf2387f182734022e38ab42c03b891d8ecbde47d1d1b64c38c4f4306274f23b8291cef830620e8db66" },
                { "cs", "0715ed5c98e240d89de3cf8408e713a88d3295c17dc871f7419d288ac751b9a8d998a8251c52dc19c95be63fc20b4e7e3c68359117f623ef86a58d5bcbc03f91" },
                { "cy", "7e3d6f0f5271dcb23616785f1649afeea1c44b94fd53573e64ee760310c70f513bbd5e33aa72900bd1e9796c7cf82eee3bf85a353bbf25e5b7dcaa29a0b92bb5" },
                { "da", "f85b3ae57be0dfb6bba3f4c321a8ae0ef2aee88fdd89a4f78ded6c523b88b05d18f3cb247dacdb487e1881634e2590ba2114e15a21198c5447e40e0d287fd1ac" },
                { "de", "00e175ec98a1a6713a3a2797eb903bc0711ddeebb1f2ea30fe22a298f21145409cb50e72ff394446107c73d61cc9c6581b9b7dbf1f9eac5e5a5baf12bb616a7a" },
                { "dsb", "3bbdc9a70fe42081d0e500c9ffef2a49425085840c3be5e1fa2365fc1ef68f03337bd0c1bdafa56db3bcc4dd446aa19ed23f858078072614942b94bdc45be544" },
                { "el", "de9cbc7457efe649893af5e800678e280f7742ddfce9935a339b4647f1a816fcfb05218f7ac50686db4848fa871f565c545395f0594556a55ae62d677cd74fa5" },
                { "en-CA", "18a3a8c31f64452fe8ac684e0e0b4a82a3585dc7870721ca0235ad20d20d2e49655d72939c0836211a5be3da52517a1f6cf0874359c02260a49c35bdb171f355" },
                { "en-GB", "63880dc86ea7307d59f2783b1140082cf83c5af6d41ff63a94242a1c1ad5ef7f1d9717a58c53fc9dc868f9b31c4f6491c51065633974add7d259cbd18ef68985" },
                { "en-US", "be19a7ba83f113984ceb3725c1c9972325040d66e71a0ea9722be8026c77617c615cc08780adedd9ab9f014e1675cd131405e13a562d21e302abbba4f34e66b0" },
                { "es-AR", "2be636738bf1fc4bb0db407483582b418ada609bc7c4b1eb4f574ef2e57bbcd4426afbfd09905aeb7dbf7a33464ddf1ae99ded66958b4b65fb32da6a6b59f26e" },
                { "es-ES", "5d24329860664675acda3dc4fffda195c1da09bff5521a3c7c45b546daaced03e85ae0a51062391e67975edb8f0ae4002db77a51fa5ac54f7a946cc673732d7e" },
                { "et", "deeff523e8b5a3401b09ee7f8c0007b2dffd322d294a08d83f3b9837bf4b751e569ea0aec2bba945fe19f9751ba6b96a235d30fae3fcac371c508e3b4f3b7b6a" },
                { "eu", "e73ac9507510b6d06cac899cfdae4720f03e9c43d082b84d8a0b635d0fed071b4e03c5f28edcfb01bb500d7f47a883257e5c5626880a489d01aa097942663bc4" },
                { "fi", "521bbc7c8fe0963429271630ffeea20cb0b5354e6d3f96e1ba990de0f4e8279d07675ee58960af70371daac3342ff0c7c3844d6ce1834d310fcabefe7205f67e" },
                { "fr", "c360a56d0d7dfa09e25616a22016a17d2c6bf142f6d81fa15bb3539fe52fb1439f86f982a650564b7aa0ed8f5928f90c7269a4eb910839034c841685d42b1196" },
                { "fy-NL", "603825a49147f9d704b85030968c9255cd037558ff3175323052634443f73f4b12caa2deab3a63a75cea7d9797db05120a0a3ef9230b438b265d30e50b39ca56" },
                { "ga-IE", "4dc03082b03c8b8d917e49e48ee0cd378fe6f70f264babcd2fece33ddf0975acaf32c254d2cbcc8e6f39e6d9438550c3e5a769dac81017ef2a13630adfa90b29" },
                { "gd", "ae65a54d2c8a7c2bf13ebdb1023faab78d67b1a39594286dec5211576c3d8ddd9eb3d309a948a2bd1e1b308291a258b9bd33a3b2a46547a3f82faa421b89174a" },
                { "gl", "03b026f9d4b155b1ebc65b85964fb69f2a1b9609de1ed7541856a070e93e3d498e5e920df66368ceb4e9da25de47f4bb64fbf5d252de612fbcdb63437e70643c" },
                { "he", "c77c7c0f30921327979b89e48cb7d68f83581634a0c0140c2127391fef30dd70eab4bba3703bdbd70405d31a200cb43c1b2b937243cee62d1df5f3618f88f853" },
                { "hr", "c5bbbcf60ba08ed445774d549e3db732601c9bcaaa48a664532643b0172c18998f2b5d9ac8479acb8352dbe6db0c924716b9c83a11b9ccd2f5ab9f14bd8472c5" },
                { "hsb", "6c3e2701534554d9b4c1748ad492abe7e863ea55f37eb53b09ffc27260a2a15c9c0adb20a1a2e2668376753a5eb920c5f52afdec5d27f69e764e989d5de28b1e" },
                { "hu", "a81a2d01cb7d9831e847183e4ca850e50bbb64e559d0dbd1d7347381b3bf040b56b2963e658fb8089fa1c89838d62821febd909349c14319ca2c96c11ef239dc" },
                { "hy-AM", "e0e3593d0ad1aa208ea576f3a4a61fe5657c6b7f21c29d78be163228d1c76d9cb28d5e1586e2dab10717715ac1d415af11d8d25a256ebd22aee19d1e85033f5b" },
                { "id", "b791276a8e3eb032b5ef7d5bba151bbcdd850169ee8120988150a67f36217db78d693b3ef8016e4a22477f0d4a3d0d923d44100984bfe8e20255b0683ffc508f" },
                { "is", "b2b0964f2530884be7f8a4ac7abef5a408a2406515d1730ea822fbb969e71df4f26eb78532e16d942a29695403f4819ff30db6253bb7150e191f38a954745bd2" },
                { "it", "e31dcc7939ad3601326302690d5081444e64533568a986017f3b467f85435053c4ae1591696417494bbda6f21d156f5eeda7539a31e8e6db1b210c612bb20732" },
                { "ja", "01021925176b053604cb800a60858e614bc610cdad1989854e5557800aedd0a4e258389d5ead33c9ba3f57f33a0a56c2a5450aabf2ce8e01413142150df98fdd" },
                { "ka", "bf29ff41aae804e6c156ee91bc10cea7e2836f5f0013d5543a34b23097eda446e5358e5dcd7dba24501bbb9aa67e4f23613fa821cb65990517c7451ccc00857b" },
                { "kab", "77b2aafa9ebd464ef938a90434cdbc9f2a9f516a235fae00434bfa76fbf63dd35add8c338dd36234680df1a42c0e64c307431bd3b9dcb89bc9728dafadc02bde" },
                { "kk", "8bbc58f59c672a738f77e74066750b66285fad871ced1a29e664265c093bca7eee73609e51831a0c3c1a7fd8e07b0881152b06ee63e08024f8da2d00f4e4380b" },
                { "ko", "18082d7c4e13d6c2ce43c7defaa8a238d57ccc9584a5a332e2c6e8cdafd7fba6ef80a61d47f6b1a516af5180b64b5030bbea836eb820c7c71a6f32eb39197383" },
                { "lt", "8a7c3c4ee4bd869c7b3e22a5fcc44dec1367c4b5e917d6e163287cd07ee9f1316bca4da5b1e3ebdda65c993ff66343d15ce444051c0363599f5933bca96fb2e9" },
                { "lv", "e135d8907264ab5c45d8a75ba93a40139e66d85e61918fc56b66baa6ebac04a7983cc93da910ecd74f494ad0cf797079b9937e0ec77453750d891c6470801665" },
                { "ms", "52269b6f7b95afe51fb1ef9bc0d0b04c8d39cbb601dca8e2d06433a45dbccce447dd4be061000e6e5e7b94e1d9f76132a48de8c36f866b776087ec27b5994789" },
                { "nb-NO", "c091173ac0d9a315bf9d3860723d7d00fdad6610a1ac2819cf726714ab51c75ac5ed2d87b0f979b3e62eb7f0f35ce1bb16970143d70ed899c8f6976a0d9b4c4a" },
                { "nl", "89130bc96bfb396b12de568ae1253251f09fc6b616680b4233e9a7c8a282d5ce5981021807b848c24955f36d0dd4e4f20b286b756e4e2b786758fa923e388413" },
                { "nn-NO", "42a7877a35c78e1e1cbbf3f65aa5fccfa0f5ebb3862865e228ba57c53ac91eee9c5259601813be7f245eff2e81456c09309240775254d4a49a127ed2382ec316" },
                { "pa-IN", "ec8ebfccf531ab35b65c36beb6ee97ab731c271be542892865d1505eba116b3cd87e3da58b8a5ebd7704e7bda274b95e6bd93cfb9b0bb56743534226177d801b" },
                { "pl", "538078275a7799523d69423812f1b70cb88725ef8512e3fce6c2f5f24dbdb1f033ecfc87dedc038ef448b16a0df3d08bfcbcac7e012e82ec7fb2af6a2c6f0b57" },
                { "pt-BR", "afea69a034ca248583079096f1dfce16349f400eb52bbac71adcc784f7e24e1929783df4a8f9f8abf8f15c81ff2737ed67c22e9000c9c9d16fa690fd831fe264" },
                { "pt-PT", "1b77169d98dad7573f70f11fda1013372ef015440c31505780c62db676212d08869c82a9e068ce1c25e3079ca58cc4a259418f13a2acf6a00b9dff8375a89454" },
                { "rm", "8af5d5b2ae6301e30af5d0496c81bacdc55a4359873287f9fa94f0be133adf606477569699f8ab2d0b9cdc175d94c290f43d91fb64c2dc83ce6c52bd12ab63f2" },
                { "ro", "2b1e39e4cd5d532dd6bf87ab827bdec45455abd846b6cb98d7a7fe3a6c55429e6181fb680264617153e52c3955170edf5cade09dd5cc819fb73128c1106bf20e" },
                { "ru", "afb405f8039f6c5f30735a70102e728d59cefb5a6d9c2c4e13c9005ffd90cdbeb7f262b81ef0f1a0ce2bc6e3238e80b4eec6ead025c40a6a773677153c6e2205" },
                { "sk", "c61cf06ab207fb04d0fad07ecc78850e274f1eeb9da9b2ea947d3e4e3b7df8748e45ce12fdefcf81a8c2917d8f8e51a1f59f189b599b53b8f0fea0f4f0709a66" },
                { "sl", "d28a2edd8a169523dfcaa892d5e69a27ac5c628ba8b586cc12aff7d2641f8d297025d0d97b5b51cafb46b56e49e9a7fdd398190ebeb4e1e085e693a2df55bcf1" },
                { "sq", "566f1d718359fcf9bbd06d6c653e3ac31b99885c22922b6cb8fdb217232f08cab74a5d6e27cc06c41bdd7731c00b5c63f4cf7ecc5367b6a955f053e46977c477" },
                { "sr", "0bbed200076ec0a5da9e30573052ca979d3c937e672408ced561898b1796859e513e5400d79cf1ae0beeb59e1f6fcfaf89330a9612875de8ba3e19bf5180676b" },
                { "sv-SE", "1c3d1dff76a3e51ddf1aa902b9e4385d14292a1b0d88bc5387b862f6cfca03fb3fceca6811f9258c0277df64240e741e344a0f3bc5230225a572fe859a23343f" },
                { "th", "ad2ed38e33ea89acc4e472939f6c721ef7d56f97ec17a784385fff232ab3e458fa1eda9c88e464cb72c8a78d6dbd364515e4f6812bde107bef1c7b44493bd322" },
                { "tr", "315ac48ddd2e798b707574b2c78ddf2d6e9116e596f33c7160800b26c5efab2d779fc0760362fc04fcce0a031cb30b110c22f645347f7374be3474570eb29b02" },
                { "uk", "4a5321064ed8bcc542eb74daed1d20ee23575763139d27e7cbaed22ccd4200f52b7e952bf1f1b3631bb3dd6b6f34962612c3cc0edc880733888e4fa6f38faa82" },
                { "uz", "488a5eeba2e848d0b8db9c66233b68b22eae6dd1cd9035a8f498ea513654d8192eb4da964b80e0d3f869560ec10d3849005544c3f13aabbb27a71f4015e5bd4e" },
                { "vi", "d6128c61cb6903b2a88df4660e07a3accf4390364b3b644fb6057e8a301e587126a143e00878f5839932b05c4b3fb3f0540a983e4c2db24c56ba5b6cd90b443a" },
                { "zh-CN", "98d45440a5d0702c0c5c27d896320b50342d8b20eee8020e535f1bf657c6e4455d43e9a7db1f3dfffcd9f4b143ccf8f55f77873174025840ce0b058978206434" },
                { "zh-TW", "9622fbb86e0c22b6edca7dd2f7f6bb9db68eb2156a25b216979100efc66ba4b70327225d7bf71333a1dd7e3e51397468a454b30bb8ed4b400a5ec91f36ad9fd3" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/91.7.0/SHA512SUMS
            return new Dictionary<string, string>(65)
            {
                { "af", "de0392356ba721aee25fe4a532839f04c0ce0129b0966e0e97373667e67c8050d4a9923180c10bdc496cd9fdca53e4942d711c436d15243d46746124cba4dad5" },
                { "ar", "e392464e55073b916b59837e1133bd710404f529aa2cac5aafbf6e51d1c03a8bded6b7d3119b6b65873bdb9c789dbb3d45034bd776de00b5a42c4342b26d912c" },
                { "ast", "5302a0b65fa4b6d32c94a90352830603bea72a0b1fa71955b54f695e77ee2a16ca50346fc933dfa6c430da573e243167d85c04e160954edb54f3d041c2219768" },
                { "be", "067b947a53673ac38f91a92ea65df6663f6d052b70041cd00954902005bcb3b2bed4c7218c5cc5729991b3e0af011bdad47925cde23fa3caa1403f3d2898cfaf" },
                { "bg", "9d53fa087f27726b0324b70a065b2a44d92ae6e5ec0555b8f786d039e7d71cccc9aec27f5789ffc0bec16ef176ec553d2c73ad0ec53af1d230d5259f847d7cb3" },
                { "br", "a0397124b4f91d5ca5bbf62c0bf0b7801ad86eb02979c71ac8d7c0200bf02e710239870bd445d663a9b037c7ee17838298228c8dfa6237ed8cb70213d1794287" },
                { "ca", "cbfe303b5d34e500e14eb316eac2375682ec389e7aa25e48ff389b5dfd94b31f410aae469bec9830504b9bdfc0ad82df97e827198eeaae7d8a41200ca580a3f5" },
                { "cak", "11fb78b31f95205b17b4ed2e815e84fbd34986a3f5ae4ed563ada273877aa29a98c51f5fbe07f3e8ac50b370a27ac46441eaf697605e622439eec19c05cec0ee" },
                { "cs", "42153f68d371b1b99ad9a1838d8573ab67fe54278b6b9678e7806ed43e94e109c0bf89f7dbd8bef4fcbd382af2f524abf720798fee19c783e7d9af82e7d065fe" },
                { "cy", "f217fb797227833c5f6134dd47532890dada7a086a4579d22ab04d487a93430bd98c6c13b127643d2cfb4c27d3c2418b3f9f9c3cc0e2ed88bc8d2c61af0e571e" },
                { "da", "5ce84292675ab5312de93d3ba0f464d0cceedf00af1d59090d0bcd410b047656c0e1bc09eb046a9b5d0e69b09a0c28e9e734e5625b828c26c65a06bf456e3152" },
                { "de", "9fbe89ddc6558630d6dae91f54172301b0867561db5c24e9e31b405c37a9e8f0e2f8b4e53e8d54167c51b2feb4383cb3b8c0e3dc9a569418ca434432ac209757" },
                { "dsb", "22b98832454e90a310eeaa6efb205eaec5176626c3e9632a48574983703ad53c84d784af05e431dbca9b1db9a3529161dbfd963ca28692e44f0826fee37bb7ae" },
                { "el", "2f67427fefd59f996ba8096c9cdc933d9a882f3bf080ca45ce2cdf3fb0d2d99e4d8ffb1e50174725e2d24d2dbebe55f5c413691d9182e2ae0b606332ccac2c1e" },
                { "en-CA", "ac003baba8b079d4af70f2b5cde751ff5466ef5c52967e3251e8575bb17481564b9fed5b36cb1b087906f57d16917bd49c02954db69383131920bb850bfe134e" },
                { "en-GB", "a4f3fc0c33d78988922533fa05b00a39498e7ae2995fcb38e6ac1b366b8ab3db3efad9580678e9a66777694c142d4b89447388f1f1daf8bf32eceaf0ea228a92" },
                { "en-US", "12232cf10d0009ea651ed512b01dd63b75469f3f7e8bc99b88907a506079b2f4a4722008e5cc7caef6bc08695372d5d3a66ec271ce81c1f74ed91603c6796b3a" },
                { "es-AR", "0a1508efcef223e4a5ada5993e8a72b804621666896fa0d3c339e302effe5e6098c44174a803ba1087513be73e0225569d0166ed44cdc29281ff00d04bc1fea9" },
                { "es-ES", "a6d95ba41707da0636fbf86879557b8b2ffa1f0024f71f2980d6faec94c6cc1f3ad9373abdf4b28b4946c204e3d678482ae9fc7b1aab8a71c647ceb81da71ace" },
                { "et", "4dd83f2f4d2eb32881cb50a7fc48a192a20d2fac800563338ebd2d5742f1203949533739f032366e5ff5974102bab43cd8fcd8d73a0d27094ddfc30d277ec491" },
                { "eu", "f36f107d6d96f56107029791de00e135139be0ead804d12ebfd055d0e0a0ac4b9574dcc967eab15621e1285940b54cf7026dd92f6717059a31446720acbb8415" },
                { "fi", "370e2d1eed4bfb2960daff67c29b9e1815e186f2e18e8309ba26e0224555c55206d9c84d22de65903008a4e9e8c943b950a419c79ffdb86a20087ef470aebd4e" },
                { "fr", "345d87e1afcc657cfc40643a94263337128767964020626f9f81688721e3a8b714cb2ab3ecf681d1ce4803540f19ca4da1a0c0fdc875e17daeed7d5e80cea2a5" },
                { "fy-NL", "eac3c8fbfabb2914490cc4546b66105760693bd23e41fda4208d4ccf4b670f3c2711b58f533ef91f3f568d3df62febb36cc7ac82d9ee936e3dd47053c97e1c7b" },
                { "ga-IE", "da398842ef739e2b5a0836451c738927213ffc03b9e9729cef08de6c51ba4f5abf14c8b72e26e7e94384c9d495f2f4705553c76fc7aebb5d4dde8636de0d2fe2" },
                { "gd", "9eb1258f63a81d67b450c6d2fa28e62e5a28cbd45a734236548c35d36f9f574ba9be93b6a73fa17c8916f46fb3ae8ae87a99ed718dbf04e0373715442720f6e4" },
                { "gl", "1e7978b172a2681e4136577602387c330ad577bd7a9797fb5ae82bd93f4c11970001278aff39a817a401f8f0583ce9109d2112f00453ba75c5f81f330c17967a" },
                { "he", "47720aabbca2d54fc146713668df2eeaeb6fbc608f3818550505dafb456265e368089e7027e018ecfc30aaa9db97d94601e95a1cf6fbd0bdb4a1c837c2bef017" },
                { "hr", "f7d4c5269635336f9757e7041b3ef53d706772e3ce9c74a55030400680db49bd6f6647dd02e6ac685c3af2d8ac6ccbb6149a2ada134cabe490ad2f231d9e70de" },
                { "hsb", "323e52134012ccd997d1837b439d84d9f804fded1322772667d6535d3053693334d52abafa20223230fe307a48007a95c249d8d2658ffe458bb38b2b64492f3b" },
                { "hu", "0538b9b9504dfc4d9c54dac158a2e562592f271511845ade48f2197c9c6c34c076edd78537a75ab5e3298043aa18e78af9e425cdd95c96c851bf76f0938364ab" },
                { "hy-AM", "afeaa67b9a4a362b957a8b62df4dee702c0299d24ea4c02bfb95766ef38030a5351937c7fc0d98e24b7116ac8b8d584e37ba104d03960547da1cc81d42f50795" },
                { "id", "aea570e672c15eef45514ee89f7b87a1c8ef1a35b8911ea6907a69e2f47cc6bd61740cbfeb4a60d0a482a37010876e9d25231bed3765d996558df8bab713d266" },
                { "is", "44d374430d9cf63fb423d40996bc804438776d5373c51b1c93a364c117add908f95707a95f71ab40ceb03ec4688b246efd095f4f22652b40b790fac005966e37" },
                { "it", "f5698265c2c63c00fc252eafaf182fac98560022a9d7852a7d5732499cb978daf39f38dbf470c95272cb3395d315553ef23a4148a3d2455daeba48a2d9d8fc2d" },
                { "ja", "8ce940cb01b2ff027c673900d0036dc783c30a310d94782ef137a8682dcd798e9516ca296f5db56cb7615e65df1a38896686ad67f369739ed913c5aa9f6a53dd" },
                { "ka", "5e1b427aae9f6eae0ad90dfc34408f1a080f5b5398f5afc04486cac1cdd52f43c1fc4c9e6c7fe45896d34dd5257c642bee204deab2044516d921c4217e38bc6e" },
                { "kab", "17bab0a19d1cb2952627a91bd5473e994a46a947ea569cf500ab13e9b7a0fc6cbf3a5752cedff8dde9cc8cd2486d6eb7d141b2c47976f69c49b4ab6480fba6f2" },
                { "kk", "4b6bd47a48206c9d1c988b84ca9544c5ec7c79ab2f9b15bfab765530ceeb6f47b0c386009529fa274fc14e5a2518e0809bc5ce98941d42a230fe2b5686cf2c0e" },
                { "ko", "1e62e5491571ae0a173da11d4e86cb98d8a83ca041702c9fa8368c9f30204ef0a23d96da824b06daecae032cd959d90556e348163518382c0f0483d076732c41" },
                { "lt", "17655103c31b4d7cce0df412d8266eb7477de8c627b738110d6736ca35b8c41148339a14eacd1a5f7e762466c06a549954173ca514a27421e962fce3ef7e140c" },
                { "lv", "4f110867bda3162793dcf49b87f3b0f2ef58dae7888f659b12910f04688903bd50c3d69c3eed9543ba840d5ead16f5f028bede1979e80fc690b7a7cc66d8c353" },
                { "ms", "6f7262490b76ea177f6af5f3d31df482ea690654817790a663816af60cd2c0ef9d01a23f48ab2b5e226105d83b8e5d67b49b7b765c12086ea00485d90fa2cc96" },
                { "nb-NO", "3c5856f586a781d4cbf7b6172d3941ad25954287c4b96cf92025f476b4d3b31215588098f0b2ac7a469ccdabd39a715009f1cc536eb5bee16ba4e928016fa673" },
                { "nl", "aa12644be8c77320ebf3e7a6927993d5e0bc60a256d05d21cb01990b0ea3040d9cc02202bf4bd68653bb225cd47c3e42619b614acf09effd26e758d62317ccac" },
                { "nn-NO", "699f338ecd04fc4d0c8e3f42307eb8344669bcdb7b6e85f03b0be7465f4c7ba99a6229be6d467365eb61dcd38605a9ac2ca696f533adbdf802524c125a85498f" },
                { "pa-IN", "5bcfcd0236a5b618b2b48cae0f1f3c07ca23d95a9a70ab2f42a9f20986258ab93fd37e7be08f2319c3e4f635c5872afa512a6f69f42abc867dafa70daa646c7d" },
                { "pl", "efe0e0da73715c4248c66bb9afbfe81fdad2fd7072f9ee30b984487e0675b73b2c5f25d254191456673464078745adbca603f7a7d0598327af5a6a59884efbb8" },
                { "pt-BR", "6be388c06e0e6d34ceb4e6f969c0322be0476a3b3b4626e6bac5a3e9565b3eb829d72f8c1e7b5d7d1b99317c399cf44516af2affc5be3dd3772c97e0be12a61a" },
                { "pt-PT", "ca89b6ddc9b78e5afc9e158b04f264a8e3a70e420148bbcabbf0c6d0ac52f80f0c595bac5ad530b60ba233e46adae945724f173a7c5a3013bada4ebd9809763b" },
                { "rm", "d36e33925bff19415494758c6fb54395dcfea2e71b9ff8d4bb2a5960ec8bb4f3ac0af1f76c35ee819fcd94f19f39ab28977f7cfca4a5e6dc87a1cebbc41c97b7" },
                { "ro", "55dd59672e2301146e1777ac243651f9713b9de350096561a4f56821a38f11d43b2e73b3add8fe3e32bdb03eea113fd5d91131c7311cce7066b2df3fd89d232f" },
                { "ru", "fc646336ade2cb28adb4605067c7b9be59c7d48f8732ba4ce136857ac545a4492550b7206c93c38c58e93c5f8e015d035a90db97de58685510480db4f5093044" },
                { "sk", "13f9438efa4c256f43864aaac4f447742ff221d75d88ee04d9819f9481d0501b8dd95df444593b98e2bc4311b6ccc92f8efe3c2d8b5b70afeeef2a768722071d" },
                { "sl", "8123988182271b2ded74d07e16e48133257310a414fe9b6076d967a778ace5bac2ce73b147d5c84550cd3191226111d504f4f189c1cdc7d8412bf4b65954d3d5" },
                { "sq", "dc11fec899d0219788c24f43edc463d3b106604d1d5abd039c6b4f6c888fd212313f7ccd746001b04ce1484ef09b609d9a609225d88103a1edad905dbe3ee551" },
                { "sr", "ac4f8870d6f4301b550636ad4e0f93ec5837de6553ed5b8ce27c426c8e18f45c31b7d54117fa29cc82135bd2ba7bec2e8c841ce4d2a3a44bbf1284a455115d24" },
                { "sv-SE", "d490b4aaddf9463865a66632269990b5c86451a46c22e15ad7054be084a95ec711847298510625c1a40a3118d34f0fc5e7600b0fc6e2f5dda60541bea429e6ff" },
                { "th", "70085ea77d198b4f6e54ce0658b38ce77349f7befc3ffd0b691abcbca3183dbb0105f64223779f12a3c523356fa60f0e7608a1db3a3c1a7fbc7cad857dcccde9" },
                { "tr", "f7dad414486e657610b5ae719c2cbd80c5164f34a1364485c5d8c5e79ece980826e38c5f1a50a86d2ded970cec8bd716e9ce70c42a08a69aff5e139096903263" },
                { "uk", "63ccbf09c5122afb125b1b1739dbbe6fe7746de9b5a5aad2830a0e7565b784b93a3b7eadb2741b46469b4e46828addcee3d13faa0fb8a4d917e488897263acce" },
                { "uz", "1b5de0aac0a8912808db9f629c077e11e0eab6042eedf73e3a0ac59c169cb4d15d67f19a3c41d2f7be99ca756ab50e8bbed368a5cf82d6a4747a2c1c46f58799" },
                { "vi", "4415d278b32c2448b8f08a127247031aab581f93a94809170b89f9011d92aaea98e1033ac7f2119b7a9fc22b9ab8287f7f02055073d528cda4780de47710a11c" },
                { "zh-CN", "9d55c67fe1849b9d065a13b8fbd7ceec7b8b5baf5a0d17d04d39e320149d0ee44ff79c24f113dda0eb1c653efc2f16af2a396939a3e244b3bebaaded2326501b" },
                { "zh-TW", "10ee777a80edf2b1ecf3b4cc7a24da22206cdec886daa096d53035dd3125fdb758f04fcd20b8a89b13da4c961484caeba170d012a31c6408d4291c5b642a7fd1" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            const string version = "91.7.0";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30_000 ms / 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                Regex reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using
            // look for line with the correct language code and version
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value.Substring(0, 128),
                matchChecksum64Bit.Value.Substring(0, 128)
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
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
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
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

    } // class
} // namespace
