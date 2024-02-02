﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
        private const string currentVersion = "123.0b6";

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
            // https://ftp.mozilla.org/pub/devedition/releases/123.0b6/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "31a78d4982d61150ad4c1fafeac7c0b7aff0be955d9bf6b68f9e6ad65a4a59a594a49b5b8e9c7eb1d2356fd92cbcf329394a56db65e5e797e58b4fb140cf6209" },
                { "af", "c7fec8ffaa7a7ec7e2b982bd4cddf733877f69c6acf63f0e29fb6c25a65906d2919cea47c7a3388c11d8de80af01e201dfbbd2b65d65abd86da03164e743a9cb" },
                { "an", "f7d2dda07f1dc4fe2945e1b7eb189b491d811f809efdb9c4fd5c3c50afab216a6a3610fb6908f6fa21d8583a413cbb8581b5786eae8ad56bb92b52a6fe566871" },
                { "ar", "404d4c920821fd06cf45fb5854225ff067e4f708be6f51ade0824c05cebea68920434f869a0d0ec92ec66c92ed7bbdefdc308a49ca24a216f8fe352814285ee8" },
                { "ast", "23f1e2b00a4283cfd98fdfbb408b34067417e6f27c95622b8d119bc034436cca189a8c9c5e3f5c05d349e7b902719ed3bb180b66164a411855ef3f97dde56b38" },
                { "az", "3ffe83a1d85e878095d004ea6d2dd920b2a8fe9d18941942edf0cb5786edafa04d496a732c76adc821d9731cd0542e72fb9acb1a7af6570f7f4f7e608ec7adce" },
                { "be", "6294d0fec0ab389f513799d1038920be45d633026a4c0a208d2a70bda012388e56b7a330adae51c0d3bf371458399cca26281138660f0c85a31abb167b3a926e" },
                { "bg", "981457c26163ab446e16efc7beae3e93260de434aeef113db7e031ae60608bb87e1ca51687b41401606c71bf5e0d26c74e8885d796a22834cc7a8396f8d1457a" },
                { "bn", "c96ea47c0336f2ed926c7afe850ddc3725d8d071022ad981cf2bd3842db72b5fdf2bac9e2a957e959004efb493cf88fa90bc49995370e38933aa59e960f1041a" },
                { "br", "08c1d6cbdcb0ca9e79c58e57e9c3535c2e15bf80cd003d865689e597bc01cf4bc5c34109b6a395b6a01bb97580fb61b990d330a1d599e6501c7a681173fcc6d3" },
                { "bs", "37d72db68217cde69a002c87cd5dd30b753ba58d9fdd393de9fe43877a173116a06facd95fa38aa42a2751e304c9d280988ca62f4661ac4c555529e6e7f24192" },
                { "ca", "afeb5f80a4e6aae1e97835be7c305ffa00b985096746b1a3dbd336089dda82c04fc97b1ab8a53d888bee5258c699ed311aca55ee562af5bd57837face776d28b" },
                { "cak", "40bb8d939873d824aa5449dbce8ecf952276fc718f5aa077d8a701e7f3d454bbae7b00da6cf11d9c40b1c3ba4935b7257471c9869b1fdcf42ae9662b6fe6d2ba" },
                { "cs", "66cd3ecc9aa65f3e9414b8bfb9b639bfa1dc344f7bf650ea786b0ae6a577e76b4e30bb7196ec28091d1cd9df0bc47a9b5ef3b54d9a5f35ea5f5d74307738a4a5" },
                { "cy", "bf54e9e7d9dc4836ac5e171a3dae0fb02d6022f908fc51d6e7cff95fab285a84122100dd6ce651c5aa6a978e0205370a6202da94e19aaedd9f672770fcc07302" },
                { "da", "c5c1c2d35dd46367056c28bb9ebd4388dbd21caf6513a0d773bda177312afe1325bcf344985e1d0b45c18fe769c65f29c520efb5598a6294d3ab10df375d66e6" },
                { "de", "f379ad284b70d0567da40e9bc5ded081dd20c310690a9fa8ae695504413b1dd9fd29d40e91cb28e20cca2090c5185cc250a61e15ec77c348bf89706dd0dcf108" },
                { "dsb", "2f050a43089ea2f2f13a0b033431eeb4ad72940ab53fceee27c726fec26929486c6cf2d29784efefa1247aa2ed628cebc4314732cdcfe440128ede60dee58742" },
                { "el", "759d5219c2f9480179ddb89357e20231e4feb0ff5077272afc24c9d24cbd0aa8dcbda586ca2f06118ccf2c6cb5c32ac2938bd5f00218e79f02cb1166305960f2" },
                { "en-CA", "b9fa38b6d19a3250a84323502713210fe801345c547c0e8ca24bfaca63d40ff58bd7232f16469420a2a2fa4abc36b427206dc8d067a0c80bc576a2363b7d7aff" },
                { "en-GB", "631c66cdb27d71a5e98283e44ee2e5233f09d1f6c5810dcadbabb9ae8ebf06546ad27b4f4099c9fcf13e500d12a0b7abdf550a82713ad8b64fd2b36a73a36f7a" },
                { "en-US", "8ec355562adb52fc59707118e4c826c7245187a0d60e4867b6133fe46171f411acbd7cd5856b8e099eb6e477f7bfadc1c2524cdba376adf90b4eac36ac486aba" },
                { "eo", "b9a0b4ad89b1e7181842011574f5a1ab0f3bc5147b7155a179c2ce628597291851cd6a5a670cca922f4b09392bbc9c7214ed3d584bc600605738dd92b43f0c8b" },
                { "es-AR", "9b0aeae1b903109ea14908318ad63e31041c78d18902563939ee3268ef3316b238070fbac7744fd6aee72076f591f9e3a2d8b2617822198db70e1f57a30b664e" },
                { "es-CL", "2848cb72a1378c9d2a08ad3ec07649b499a54c34cf5f54b8673207d81d446821151731f04b0d6c9dd7e9e0486e474634649748292be6a60d59e076376784b0d3" },
                { "es-ES", "3b4c70ec056d673930c1a916b8b5af7b3a2525b63bc204706d663294a5f43ee57f4748b7441fb31ce6e0bd0e38a52f387fd97208613b4d04aade963bab862d11" },
                { "es-MX", "b574246ec29fe2a93e1698066435397030d8dd016ce2bc1e054fc9b8d6654184eac1886a56d4ff592b05803a3251e1d93462abd264fbfc4ddd297654acb4493d" },
                { "et", "503fd774ad1cdcb7a8b0fb1e1b466163ecb33255ba954baee25168e6f4f119a608525346e0ec8ece594eaf5c46c8311dc10195b83c5c51063e5659acc733bc82" },
                { "eu", "83590cd09f758d99aaf848377a210f8f27451e9633bb43400f31af93a3269fbbb1cec1fa4752ee18c172e89fb2c114ab77d09a92b640125284898daadab0916f" },
                { "fa", "88db92eafcda92b8a8218fb15e0f8dcf09c6ceee1f6760da161eb91681281851a39a27d7a0829c1bcfa1e29564ff487ad1ef94a6f365a23fbc6a72c66df77ba7" },
                { "ff", "640265fdef0ad1200340b259686a0d90cf68e09a4184b375a97ead0b25e41d9901aa03d275cccbcd6a3acf69501043cf175746fb976c3a6380549302fe04d601" },
                { "fi", "fa33fb8f6f8ac68b600420224d7f86888bb31867f4d6af1629dc23ebf158d8de986ad77ac7db94a128380cbc097ce950149c3ef1012c1baaa762e801bcf6806c" },
                { "fr", "084a661edc7c40974d75fb213be963ff5264da09abe518783d0b3f8b9bfec405cab8488e50a816a4ac8a73c265e3cddfc28781382384267d2facef40d787b034" },
                { "fur", "c02a99d4064aba0f0dfda1b4a7f2becc5e9bdaf1bc452f5cc204b107fbf784a9a9f446b91f674d22f0bbf2ad23931664866c2edf9806185623f8f2fdad2826bc" },
                { "fy-NL", "255b451e0169574099add2cbadbc509b884363a0adb1ba4e20df283357002019f5b18fdcbe48ea51c0c24f068cecb40887aadd28fd0c39b67cdd713b47c0e3ec" },
                { "ga-IE", "431be63e538ef91403d2a2726368e8cdeaeb7a88a170c06117b0d67b08352718e0c8b9eb6db7fb803a522b485dd8c01b24353e9d40ab0334416fa53b851b7fd7" },
                { "gd", "c2cf5039f7350a139d1672e27f4fd11a13991e3168eaf3c353bed134cb0e4432b731326b1adc58da56634f758833d5831924943b5bf12b22ab1968a3bc869183" },
                { "gl", "7f6a86944977aeaf7613bcae0a57b6f1980a0fca0cb9450b728fbeb3af14ca9b37f44cfad87e3f23501b8af2653bc625a34b815c937111c8fd303627ac1c94c2" },
                { "gn", "4d79d5fbd5b1d7fa6afe0f2505a176e26f4115a5ec5e7a03c888a541ed5da1fb5508a5bc1767db51dd6d4d309106860897c3656149ec08964f95a4a4ee217534" },
                { "gu-IN", "d1c60de252f98dd4113a683169cdef6c08ff6c71cc9b6b3ffaa74cfcc9a884de96780cbd6fa7740258ade92392c14c5d693347c064a095e6d2a6e39b235a359f" },
                { "he", "f3d684ecbf81a572a901ed9a51be84bc3cc6019b698eb722bc61df811490e1e6723c60325d8245e844e9226533b05c8e0cd824a7e7454875f2c3aa755869cc23" },
                { "hi-IN", "81fab4f019b4a6d3c6493e95fd8c7ff5f9de5bf1f8fd71516681d343489b818fffcd0faafb00091f8e5c2024e761292aecb0c2b4f764ad21e09229b94f59ecc4" },
                { "hr", "13c514a44f1ccf408bb3d4fda5a78b490c82504afd2043358ff051c590875ee496042e7c3af24239817cc5b38773e63d2859653c44822f9e9672da53fb22ddb5" },
                { "hsb", "90780592f3da0b034e36775c2fef7e24fe10c5fb0bbcbfee406f77b818807b7cf7ea564e934422fe1d62325728816cabc69e02765b34cf3965c034ff1046eabf" },
                { "hu", "5816c0c03529e740df7b5492992332b44e6824348d6b41e37f8b6fc8a9b8fc15a6f513ab8a06f63cd86b4b3515c275b471ff9c59228c6596c0757c431d93b4d6" },
                { "hy-AM", "72c2b19e8803b6f43a116462b4dc5fffd0177cfc34456fb5e11b8d2b85cecfce080763f5f2509196021de8b9c2d79c6474718476afedfac458215aa6fa233c15" },
                { "ia", "20cfc961ad7ed37014e3978ef08f16240bd93fe5a041608707c619b109cb1ff64c17dc67cc85396ba88c28a06e56a0f13e2102e021681876898138c491313016" },
                { "id", "df132aa22681429e198b7484087905936e724bf7f9d0cd23976e3399a5473671cd5af915f52dda0f24e0b8f864f2fdd000975580235254c5c7024aac9abb37d0" },
                { "is", "ebca9290cef4d2cf2bb184906e9a92c202a3bf1e75ba1a4186bb928bae0054f5410ad4c415d20f14fa52436b4f851a0c2384dd84112efb7c1c7d84825f45c3c6" },
                { "it", "179285f2afe5d9c85abf9829464766916e2fdc8442cb718e69a168c70d15c41feb7418a56fe2de4ab58bb82dc870005efd7563b17dc8079adda428432d1b26a5" },
                { "ja", "8992a7070d28b7036622ff8851952bcf242a62a98a98ead515326161517ee692058e345e6eb1773dc244c77fca3394ae7d9bfc1e4c94c9fa6b2f06c7fca51a1f" },
                { "ka", "c8593bae7af5abf7164abb5517ee77e5d25a51478e01e27947dd7ab9565a9b05f3954be695273121ab0d6bf10ca9210bf1a0772ed621ad8ea75d0a0243129c04" },
                { "kab", "b99c438bafd4b292d12560a5d8613f69c7a75e4ad593e4e9c3b5c4be36b735581bc7438d67e82099519bc8bf76d3a41a5f71c872ffa576a6724fe815ac4114b0" },
                { "kk", "9601b58e7f89255500d6d461debe60a894086b6a3875919ee3fd776903855907af917ea993223dee6a178255a0962cd25a2ca3a459516ddd0eef7210cc0172a0" },
                { "km", "705e1ffc99147a66ed79b21db55420b1f8a774fb3b1e2bd739468a028d7c3bec3366ed8426d11aa62b2e07a9ab4b09db2325b80aa31bb8400263fb7670b20d98" },
                { "kn", "a77cd58536f17d14789b0a481d1e1a1275b3a66bc17bb8c9fb6f3a361b9fd0739040bb5a3eae8d7be3aab9dea6804ff4b56f4e128914831673e80ee9153011f3" },
                { "ko", "6a0ae50b6eb9faef231b17085e93e2c48a5928347afc0927c062bf5043d6dfe78fd41f6eca18a158938f0b30ad8b20a81e3d857ecd64da3a70db662d1fe8e64c" },
                { "lij", "843b7d94b1a3c0290813464656d96482bd288a454c7396887747a37c5b0dcb3e520d04943c1a0ffd6bd1fc4f2cfd8ae616ca08ce18935e3cc66d5baa838fbbba" },
                { "lt", "2119414bdaf265569057e43c033fbd4cfba4cb5a2451f221ad10469843ecf7d5cc3d12b859be3f3019dc552a9d0c2ca6551d78d37eb15de78fe127da8701c03d" },
                { "lv", "1f3efc3c66b28619405e3444fa50991bf5cb3d00f08014f272a0b875ca72959014168357366081c981e4696921f36774d662d80fd442a1ba9c86b39c9f27812f" },
                { "mk", "9798f5e9d54f73c37d374f1cce4c93c906f19dc75f47b8c72e56b5ee183ab8dc9330ca76aa1d05402e6f945a4973b981f8ae7addcdabec5e1109d8fb97a6daf2" },
                { "mr", "e842fc83a025972e1ad4eb129448dff6e6536f6952ea524ae0aca3bf1d730c0cd93921432b614e319bea867ebe9f8563ee43abf7b1bc5ab359df21a42f757e5b" },
                { "ms", "b64ef17f23103e65d848a1b1fba28aefb60ec7deea1ea0c82c4ae8c7769451ea78df2ef16195361ee7129e553aa234dd7a7b46c8ef4b64964d550c00eec64a0b" },
                { "my", "dc9714af077018dea8bcd3059da5045177ab6e32df2467603d5b6e989fc18f31127192faba7567a6d75223397a27ee47353339233795f3ef9ae17fd94c061e15" },
                { "nb-NO", "7f6d1ef41e4c5a16d8f40228105b0fcec44f9a0c0b87e101aebc83c4e940ac092051e8bf2f25b23640ae99878fd2d8de9b2b5db40ba7132a194e17d7f49d9a00" },
                { "ne-NP", "fcb1ce0e68506193d52fde39da17c48e6555ae6a7601ce8c76fb2c371a43fbc1ad4bc767bacb03e45f653a1352141f68cc1e7a4fc5038039f33c916146ffa195" },
                { "nl", "d05616a1ba57972b93ea2e0a5e669e5187f8cfeb5574c5ea5709465f7a88fd9ffa4552172683d20c9e23b25c56db477c3ce6b4333301c26d11e82d3c83f0b8af" },
                { "nn-NO", "bbb6b7310b30d1fb357cce085ea945e5c3ccb679aa870bbd9a4ff78d410268cdf697f3c6b744b79b29ec5f29396c0f11848ecd364fdf3c5f3855418318bba8da" },
                { "oc", "a7fb7d074b616796ab25bf44cced30ca1e9d418d718e7eca7abb4d47502ea473620bd18bc60b431599a3e995b525a9f48b6eba8453fec3bb9ab0732db0773bee" },
                { "pa-IN", "16fc9809f3226cca9c360d5eef7cbe62a7320312f129afa3a96d255c7b54a4590d5698a400d922895f918927328dea2e82a0ead38939af7cf9969df866288b6a" },
                { "pl", "6ef9694ff19daff5a751f2474233ea54a3c44172c24de8e2988226bc1191de85732554812bbc3994c46bd498ff0f21e0f5cd5e7b5f0d7025fe7980fda82aebdd" },
                { "pt-BR", "d90c325c3f976fbaaab38e323b721b8565e0524178899d2c115cf8e6b0fc20081e1dc4b6d6550463c4bb328ed39306d84a6c81816c3fe80bd6d2f568e3743945" },
                { "pt-PT", "3080166cedc532a21e649e3ed6eb6e646b25661b07b0db028c6480000ed4778c4ed528a4927c1b0b2ec175661996cebcaddc4ed6e77d50856bea3839bba75292" },
                { "rm", "f17845fdf6a2699cbb21277c38dd1abe4fdb013bf8c0b11d914e4feb28328c7f3296ebe95a683745458468afeb0516192c45f5b3586dbdef3d726d975b703ee1" },
                { "ro", "02a4211fb58e8d78adfb421577fde5016ea7270dbbab0b469194247611da987b2ffa3a4289f26d93d64a993578a04f649beeb84e9d56127a26ed6d89b505784d" },
                { "ru", "49391c4b9d2d9f5628463e39e84247640b4f31b24914f9a30c7684fed1ce869b521cd3bc0d8d7d03d971f7cdb03ce55372dffd0d5951581af0767511f7c7b1bc" },
                { "sat", "b827afc665ffc3cece2fde40d72f7ef674fb3b6ce72ced5afc2ded15c14652dfe843b44bc9b30236e4b9c4da6e46cfb0001995ae76af4e350f5faa63f5639766" },
                { "sc", "91674a786fb372e6683139e4a22ab75b387396bac16e4e56a5298356b6f5fddd808a3793df9c2a59bc54343f84254f5926d30da43a08c02c8185812ca0ce0ca7" },
                { "sco", "c73bd5f78904faf7e6d78418ee20b52d969ea454a3532b99a2ac8f34fe541c9df1a346a90919d2c5a1e43106c2ca7b91078ff251afd620c5127c24906d21133b" },
                { "si", "5e233259f3a4499e8dbbfa5a03af5c45966e6f7f42c18b74b25403fe792512362415b6d8f5a5a3e15b1578cc3aa386268fc29426d849d9b2234a41bc03e6e7b1" },
                { "sk", "a5f830e8ce4e97b3460d078f6202529d46d6e42a9527f2911513af1f29db0a157c1a66d9bb59a809917c8702599b10937cb26ddcbcf32194dc5d585aacd489e9" },
                { "sl", "7c7c871a663a803bf3c723fee3e8f48de75864964721b00bffca7a34f2319e2d4f25808b6e0ab931a2eb4c14acce305f4e1b1b3f4329ef774a8b1bb0b99375fd" },
                { "son", "79910994edb65c1a8a39e984ae00940ca5c839348e728127e3edcf9271c87d3c5cceb747a46ccb54dd0771133a9f71e667b8e46ccb8e23611971a4131c0d585d" },
                { "sq", "0eb92bc1551a66f3585428a5e93c78dc5eea5f23d711de762bf8f21b13f0cda0313a910bf50af892da5e206179829d1b05c5a14d0c67b6a901b1d62097dcb552" },
                { "sr", "d52606e4aec929b957ff4829786426952ea06f091f7791befbc6585ebfc17c46177a3da0a310b533c28c2f12156726d682fde17d7397e93d2997194e41717833" },
                { "sv-SE", "8fb40b74e081053f0d76adfec9952b4b1a64a886894adcb98cf9a06093f004a76a464d857e67dd3fb6bf5b0a0a73769303ef442516058847b827e61371b7cda2" },
                { "szl", "7855e384073d52d44a2a782d1427df77558083135da9b2c605a29040448a9feedc0688b66e3b9f7139a2e5bc198e56548b82e2e5d165a5e04a908a26e1080974" },
                { "ta", "73f56cf4da726f2af4117ca7626284413a3d6eec0aae67023a92a1830b0fbe44e726c8c5f1a76b1a498f60a15664224cdc3af7ed5d73862a5f46b21326eb60fa" },
                { "te", "f8b5892b79cb09d37b67d1b8abceea3c31a470bd43184d4cbf68fba45d777b98127690936131f767a28d32301b11ec5cb07d8156aa6ed4dfe26b68f77c7f6437" },
                { "tg", "ce2a4709b61f005e6b7aa0d9c34539af71c04c998c4c5412b65d0ae766bf092727ba8eabef7dd3295cb09af6251227140ea96dc329e32787f10df5c840db9a4b" },
                { "th", "a499b207850b8e305856488f9d30a576a623e92c7af046e36617bf303b9252f7b5fac93234196fed932d338c3c818d6642375bff7deddeadfc0e8ec50e6d1927" },
                { "tl", "15edcc72184201eac604217f39120f1445fdf336da8a67c566134b7e69ee66b5e078fe7050b476303f4fbdc746cf1a7eca90f49be05c1e7bffda723f2197722f" },
                { "tr", "4ea57b58c173025a8ed522e69fd1b8ebd4e32db6e5dab9d4b95d44042a2cc06e02e4ed10d0a9e708beb48ce223945c8bdc3a75441535c71eb2bba4e61a93edf0" },
                { "trs", "1af957b58623286b6f1d20ce8e6d66881a27abcc3c7a14d9e955c3a342104087b49a9666eeda82ebd7500592fa16c57ea490531bd794d916e77fbd49e3f89f16" },
                { "uk", "6e070396ff9d561303a14b75513ebaf44954fa2434867dddea72532d6ea0b3ee01b29069624a913a5a5ae76fedecf6a645ffe939c42eeadaadf9b6231429013d" },
                { "ur", "f133fdb499d7cba153d21ea70b9781f05f3feac75cf60281efbbaa75c28fdf7ebaa8039847c4199115ef4e295713294b154cc564b2b8184513df7232f51b8291" },
                { "uz", "08c6ffad45b50703f6df75d0e0167e5e88caa2e6993e605638b242a747a4e083b8851869d9d1cb9e38dd01643905566d8aa7864f385b2330453ad4efa6bcb6ac" },
                { "vi", "ee1864ff7d16f6aa49a10fa2f64cc1ab63e4404f7cd3a897c6aee071b31aced3ff1d6098acb72e72fd113e4b290770dd889bcdbe445bca7887ea4ae5acf9f3ef" },
                { "xh", "7d1fd9d116d8069653ed707f43a3ed74eab456e11551fbf799935494f639d0036c36eb848784f01401c3d4fc8aa17e9cd7ec0954cab2c83ad0c7d7899c56bf27" },
                { "zh-CN", "b425d6368fc3ee98e87c8c7dcc04ff0b6cc430ec3bd747ef853378961c8f6f387ace6e79fc68471a7c239248cc611c4c973a88cee78ba6087fa67ee0ea74bec3" },
                { "zh-TW", "21330ed99f4e9e923219ba0a2a77eebfe9de9c117a03367db11f8087f53bee2a6898ed3cc40a36edd74ff1f9d61fc0fbb234f57647e0f2a7687a29db157a53c3" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/123.0b6/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "9c7ac0f7ee03b53c616d1b0b65343c207aee75e49d85ef6fc2baf88a29edfb6a2ce61f98af6fd0f693159a6230a9944ca885db7164916f8fa0ed7ed2279d664f" },
                { "af", "ad5b4ab32f96e4ca77251d5debf0256591b9e8e54276b154dca05af47527d98e5cceee425dfb8258a1a2220d8e842302003c5cdecb7754fbe02bb8c7bb1733e4" },
                { "an", "938530574b8beff560d256ea0babb627ac3df32cdd7b8d95736ac209c0689707b3b10b072cfe6ccc72459cad894fdf4f61cc9540d170f9332df2c45ffc15b493" },
                { "ar", "f26ca7b8f78ffe7024bc817acfb8980b0d89973fad1d136aace9d4aa6a1eca6a98459728195e6a05fc8c88790f7945ac443a2f0829e7c4cf19b5b77ef362a9e2" },
                { "ast", "072fc7c15f9c02dec331b2b07bf61e434a3daade26190b9a31cf5fcd21589981efed04946a963daf6f14a9bd8b1e8d86ce360f0b723f09d545e152d5be5d9426" },
                { "az", "7373144fd481f650c9df3c8cc5330a4842581e7cfd275b1f3ca5daa22d2a8dc892c08df7d02e037921fad76ffe1c48fa2975fc8b449e565754d7a374cc524a5c" },
                { "be", "104b8053499c01392bbcf1cb712f1d54c42cfdabfe9d40862773a0cdbea11976d05adb6e1fd14286f7b9bb0ca2e835613a8a7232f148af404840580c5401c739" },
                { "bg", "0a95abb53611a0af592bc41628bc3171956310e20123baa039074579bf42e7b003977e7e48814107da3340303780872c51a1b3c2d4285a072aa90c7e29058353" },
                { "bn", "e9e1c88290f8bc7756cfd02863870b8c55c4bfe29cc9f321794b0f9a9045cde4bd2a91c19b62bef75fb709ef0e2eb3431f09b0ea8ce82c72d781d23ef84d8737" },
                { "br", "44c609503d1df8c96795e55f9906530c970821a405a870af3f2eaea88102431e3a67ea400d41b0ab2dd93fd1607413239866617e36940940880b76682b8d2f9d" },
                { "bs", "7aa7ed74a5d2b4db0d0f52b36e72a6c83b12db3b662aa172b62e4e53a8fbe8d0d3150b41d4f69fb5dcca19980e22692fe60df4ad75558484ab171aeddce4dcba" },
                { "ca", "a80068bc26e9786a0439cb75be0564015eb3ed23976bd37508ec68bf46c1b0d6ec88083b9a1713bad6e316ead34a1226b4135159805aa4bf8ae1e62ba202743e" },
                { "cak", "7d1a34b1728317eaa03797b0fb42cb7eb7be0f96a686393ffa641c64ed6f7da03c8543efe2674e441f6aca2ac52e282b7e65a347b7e41c2a4854b23d3ba9e4d6" },
                { "cs", "6fa7afbbb1452e2a990aab0547537ab180a9661adc39108cb89da190df3eda0ba149bc91c76355369ec279510a48a3e0295d73cfdb37b9576551caec530e2acf" },
                { "cy", "9dedc44862369ed043b478a7a6db4d9da906c84ef412784547184c370c485abbbab94dc3905392255a7d0eb82d0ca9883174a2c834f451ee5df2d529d6869728" },
                { "da", "322fad502800c72a41f29621c751e55202c9aa9657a474e68ebb22b9b8372182bf4e2e656d2685813578a4fe28e149cbe34cb16be38d1be5bab6ee591219cbd6" },
                { "de", "e2f36a8a3b2f169123309b699f527077002388f0acdcbc5df667203f84e2b820c0411116bcefd5473d6a66fbe7753e64df0583b2ff3b1877fe83d88ce079cf38" },
                { "dsb", "b8648aae70c8e7b80d82d4fc663dad82e59ba92eb4e49ef9ae4e5a32fdcc13bbcadc5a1b3274c9169419824fa7659617af4fdd5674d8d383c35cf983fa83bc9f" },
                { "el", "d716c3d943941c7233fc3e67ef10382d000aad72657bca649fbb03fea73effba2f61a8a00b19c213bdf3747f00e945d19c8935e412d6bf71fc58e1ec77be7c11" },
                { "en-CA", "38cba68d484b66de429e288dd0ca302e7ff289325a960ab2d12ab0e059d857615de7a8601a57fa29be033abb68b3cf3b16723ca655ea0a0da27e61fa70193877" },
                { "en-GB", "68e83b14757e5a35f4dc84d00dbae4f04b02728c3877453a339d8ae3223cb81c1fdae706dbf30eb33cec77fe9cc1ab86ddb6430371f2fd19c44ba684c477a44e" },
                { "en-US", "45bcb2a72b1b1cf5d23f658694dca38937e204d34774af66f90dd0f0fc646ec077780c9f1c5b150dbc37e70f69720c37c20147e003f11648440a54fd7f8ced28" },
                { "eo", "28c05a54236154fb344f9ac8515908ed97acedf674b3b19b119148c259d4ae655eedd2fb91f8a1ae9ab34064cf4a58fd358676aa746d12d20dd2a234de0f2ec2" },
                { "es-AR", "9878068ca9f0c0778133aaea2bd58fc218df4d15c6f0d86897f8257f45d59b8bd4ed077b83a3f644d4d88d511bc285199be1d9cf2eab98d71de2ec924a58f9e6" },
                { "es-CL", "f94125617703dae9035dff9be6e421a935ec3118901dabe69596ad2c8213f89704f3db7f8050a7e180d73dab44c78cb5abc589224dcd24189ddd32cd214c22c4" },
                { "es-ES", "91ed2677a7e0bab258705cc1c239781a8faf385b2d7a49b50e5d9a949370d6093861818a56be9305710bb0e1833508ec1fcc5cb59551c0d48c5491100a513a9c" },
                { "es-MX", "0c33e14eba65f746f957de84688555b82a4d3b9b6fd9d2400e201060c7bd33f0f84676fe4b144c5fd6200e99ac1d3498845c30934ffc0988cdb5920f1ac3f5e4" },
                { "et", "36b622adbc79768f2a2ce8e315b5946560a242ded6243681158ca4315fc50ad8529253593bd3303b9f6637b6a1b566bf7a4cfe8ad017b878a5c3b22b565e2310" },
                { "eu", "f6441fb67b233c9670ff950784eedca3e08db121c32c9c2aadbb0a63abb782ce30ee86330604444d37703d157c0445b4a1ab7a8c48802c3c66153b6ab87f2ff8" },
                { "fa", "a9f3141a89cd0e60a15ab6b601692464940b69132eaab7343671b3a5660d1cd6146ab9bfc7e4724935de9b3a7073d716887bfb0602e619284e9a69c63676d17a" },
                { "ff", "cc40928c1f4c92849329a3dd50d457fbb68731c855c4ad0f4ecdbdb8ed7561d6d9de649320bace1a17a0a2eaa9b72c5991d46f077f971b26af50a227e0ae2230" },
                { "fi", "d85d7d2e1a2c9bd869a9e39a55a2c697e7b78c456789740a5ed1d89947ff93a657a8197ced72ad3b3d1e03f4a8d9cf0e45f1a0ba2f45aa1b1d1a8d96d66564f7" },
                { "fr", "cb8cb2f1600ace6d243ba9a416d30ba98af9f270900b38e4d46973bbad33adfb67342362343178d3128e2fbbee8562af4892fa24ea30fc7ba72c60c8e80b7dee" },
                { "fur", "dd57d02ac0e2418b6d10b159be087884f594943104e605893600c310cafb6eb53473678a1b1962ba98a5743726bccff88908392859ed365f87b566156eca3860" },
                { "fy-NL", "d022a7773eaa633efe220f8fbdc181b26e1a35d1fdda1ee245638c6497f50ce1680a3490aa1d3f44a13749db742082dd16589d7b44ddab55dbf8091dfd38d311" },
                { "ga-IE", "a981b107ab641e54af27846b88781b29ab8bcc33415a8fb2535ea21332b7658379d1f119870798838ec93fb55cba8fd45dca0adbe9368cdc88ebf206630eec2c" },
                { "gd", "c67eb9ab5c11576a19422e1ebaefeffa55e65fe559a73bcf08d150226dc4e4ad3314f58ee18145ae2603a9239477d5b6d7022882c267e2d5667a71ec1e114d25" },
                { "gl", "240c2961a50600d6808051787074f32885a0cb66879bbb54285d30b6e61e70a166543a043840b451246873407b8b256b78e7e11fe58808870ebc12f3e341f3f9" },
                { "gn", "8e0901cf32d1d026cfa348e38c4df6a9a715c954f1bbb43d054a903202818ff5a178655bd5fe0f8b631310801a3e507516caf5cb5ef9d423f2d2e88b405878f1" },
                { "gu-IN", "c9b324cf91cb36d54f7c1c71d6a97461e947bc0302478a18e5812d110c332977ad0e6cd281226ba0b452f115f37270c486b156e1b830a9709dc794c02412fe1f" },
                { "he", "7a0ab42741bca192f46e2f6f50d4242b2c15791033a2e67c1b30c5f039985ef556ddca5ecc31e0f96a82abe8560241560bc177370f866aa31988e9bcd20185ee" },
                { "hi-IN", "f4c8efbaa5f375ebfff19836a558f433bcaa14ac37c7593d90cf0bfe58dfa59041c8d236438436c0fe0a6626fdaf81ae949490255a51ed776b92408c6249a6e2" },
                { "hr", "38367ee5e1110a1e978cacb2c0d85ec5182d91878848f98fc82f573c81a3f267696706d4168f2f2ba6c9e6357918bf843f699ccd235035279901791f3cbc5b9b" },
                { "hsb", "d0a6f9f5a47b2c2bb26972452aea750c547ee16402b99a62cfaf59706acfeb46d1ae757638ad6dffc0e82c1ce675e7a6ce411213caac1e84d28ddc9e92461cb1" },
                { "hu", "0a3545882b4111f680b19681fe8b638d96e6f438ef97fdfb0a9cae0d75817d3bd338d2befbad65201c4d4f7f6745c7131d55453b6e38e26e45059a7ea3978d0d" },
                { "hy-AM", "1c6ac65780c61ab882feee6f277d21c232d992f70460dc554a1d4dfd3f574eafef9516cedd99b404a90226f42e9a6b13f3d209696f2da8a6abcdc966ef11aa74" },
                { "ia", "4a6b9832fd36d45a3c367f015a83731c686f747462f1c2135f461cfc5727cdcef922b02f84b5a7e7cc1cbe5244eeadd3b0a3e4e84671c3bb576b364a86fd9d6e" },
                { "id", "79f56d6ed76916b7f92b147c0f4668b244701fe621f4daf522ed355ea3f7a2ac01e66df823588ee75a65dc4d08e9bcfcbb4a114bed5a52ac6f3fcba37f7564c5" },
                { "is", "cef014f8fd3ac594c9bda0863fcd783ba6bf29cc1eca24e68ea49528dfad7fd7f257fd7013b9b492dca7335fcc706cbdea614de75e883672ffadd9e440a9dd21" },
                { "it", "c9a0847520a7a37d9e25e8b9a09870e1a61a00ae083ba5d1c0b19097835b6b6056a093799a7e524ebcf56fa91834180d70e6b6220aed21bc9cb7e038cb4b6393" },
                { "ja", "016ef8091768f3c583165d6d0ba66641a67b2a457231ab68d6e6bbe028f41e0270154b64afbde70ab5ed5c9d9b0a07d11e130c1677802c33db7d1bd60450384a" },
                { "ka", "b496b1f74344a87327ea5e66cf68019864ddf569306216f8bed00dc21d4f5e3691d390c939c4b352a97e8d6b77f948f67b7cf336dcd3a405f02a5f19838eade9" },
                { "kab", "2e7755b0c033bdc430f02590b30a2eea6f27382ba00a0eec615eb386bbd5a1db3f027c3ca8b6be14bf0e4b562e09baa133ff3669b81222333066620eada395fc" },
                { "kk", "54ea198a054764b2f58105409c3777649f0496fbfebe0a3cb1dffb28617e1cf598d12bcb47811dd5c7a7c8667c04e393948cfbbefc5c7e31eaffdd44a49345ec" },
                { "km", "69f1475c2a0f506664ef95f2e635fff53624f712f223d7163f406a3ca9f55d53c4a74857009d0aa95ef761d18d4e6c32e3710530d133ec9697b45d1f9f31414b" },
                { "kn", "ee1b7e52b56fb182b5aa03e6577c06fae11375c93d4f2c6b26cb3badcda419140d6cb37f1d0d790366754ce94c8363a6992fef2d9bc516fc77de895ffbd8a51a" },
                { "ko", "364ff0c9f67aee72619311c4dfb75c515549efaeda90b6c47983fd1dd9f98cdb282b7ede7ea8a516415c2994933e2173e45b3fc33c82b9a191958f3213750cea" },
                { "lij", "f89a3e9b99a6f448571be6154fcdea3593a6143bf00faffadcb41a3744353a98387fd527da219c4cc02d2207b08ed72546d49e4c9cdd39973c34f1c8ca67310f" },
                { "lt", "dbe93df86da600a56e268f0668894a672ca1e2a5451a2eff791bade7b51992765b104751f5928e69dbd272c232749795a387478b50f6ac854603d3b1ddc27153" },
                { "lv", "93eae7664c1024a2fbe919ee1730b5d6e288b94a1f5bc2836303ae808412f54bff9d167cc753e0aaa406dd9af7ccb0e0cc96485c7fe9a777af86a1efcf80a007" },
                { "mk", "bdfc05097bf5bf21413c503e58905c3e966cfb59592200fac097c50fb6d1dd91ffe863cde1053aa265e7129f37825709866938f8d8ec968349e474ac0df8f53f" },
                { "mr", "faa78b8dadaf2f01ed7de13e3d865cbfc386391c4cb336a22cc9746e005c24f037e98eb07211dd48be758e59a288210218d318205a5694de5b577340338024c9" },
                { "ms", "02355ca01f8e120ed846b3d66136ee88538ab006ee55c611479f6e61bc1e59c1b12dd5027c038baf2362fb9698125dc3d0fb810e3c35a7b91f29d0dc0a45ef87" },
                { "my", "40af1bd13c6764b3d761194c779e9363f5c447bbafc1c9e2bddbc41411e3af3de34cf0302493ab152bb260a015107f835d04b52659a96065e4c7f5ec78995b1d" },
                { "nb-NO", "c985e1cbf3460c640c17201e0203a2f01c7276f63990b7538c46ee4d283bf08560516dfbe86e0081a6a5db36483b0780a7f6a1f45425083e9807cec7a487ef3d" },
                { "ne-NP", "5a0cefde1759516ad470a05167735e2cb47c1180a16ce6f979f34814175459c32559d815503865ae58bf3b7c53badcfadf8b89fc12f1e3863fb7e16949143436" },
                { "nl", "8fd7f9b75847d933f7ba61e68157e694319a06237ae20a4f51ac22a670ea3bca9094b619a80a8c485aa163656d17772d55bcb231ebc6c2de0b83790671e940d0" },
                { "nn-NO", "58aaf7db2eb5186b97527c4c0492e56ff90dd7285c2e15462aba50a4385ce5594857384c575884324872280545dfe2189b8f6cef99f31ebd5ce045ff964a07f2" },
                { "oc", "d7276bceded5e90422405c64ffe9fd1613767a33762c7ff66248cff1309c8d7e60cf54cd519c52d42b555a878fa4d7edfa14dd4da70a110e273f8ccf2ba42618" },
                { "pa-IN", "bf14e2cae9302880f5beaa3ea829700aac151bec6f06af3bb51e3317d5b7c35afbb0537f6a09a02297c576f55f6e95e14fab6a7f4e9e91205b96873c001d0eab" },
                { "pl", "52076542e0ffcb603b99fe11ae0d58662af021479a2e13ba697141c54736ad87648ff3222ff765fac8d0c5b54972fe54e311db2dbd4fc37ff20abc9775909477" },
                { "pt-BR", "281a8d5a3204bd034818e6b4f7681731b78712a0bfcd9994ebde02c44853c17ba202b0682c80be425424bce8af2287b013bd4992eaac7ab8714517001e5aeed7" },
                { "pt-PT", "da6fbce67945e98999f87834e55b0af17d34d72384df963c129f1883619f2114775f9d266aa9562c2cf8b316386bb6600d3b6701098e03f89e891b18f275cd9a" },
                { "rm", "2b60305fcd3de5de9949e20d0ce7a001683fd65d011e3b10bc1a63383b5166ac77348826fe074cdf7ec353411ae3bcb63d0861f98cdd2a44ab5bdeba954cbbfc" },
                { "ro", "67a6e22e4c87b2dd15acaf99301fac88e311e9af9f9185e0c0056a57b296e01353c21abfcb9f2c55faf5ae13ab05e8559f3a6c45859c4a6ee577265a112f7672" },
                { "ru", "2642b273cd6cbfc64803cd15c87e0fd5422b6f11e1b43d2da976510a4d98e20922e72f3010511343224ceec65c0f6c941c57d743f9b968b5868f6f029b9c1e70" },
                { "sat", "0792be48a07a501ee92e001d6c2c24a140d022908ba3b890b36406d1725f0b5823ef1b753f2db4235cf5ce6de2ffdbad8a195de41c3e5ec140403e277d6d0501" },
                { "sc", "69d6e5886b17e2c4b62f0fc7e94ce0a2bc31603d26227cf5add23751668400b679358117dd6b898a487a7182f582a6d75c7ef077ff7ad9ca268ac5a03f3891dd" },
                { "sco", "bd2dfa6b2769842eb8d618365ac8cf63fa54be5a4600af1b792d8ad3844f4d7ed045838e987916d9762dde052a2f1d53b19fd28072f3e0c5a607a71fca274e92" },
                { "si", "c19d9440c0f6b3452285127be6f9ae8b88525b67238e97889307a829e3a4eaa7a9e7aea755d5da467b3f466bd02e2a0e8b0ca808a4e3fca666b9cfa1de62fcf3" },
                { "sk", "fe30afcdf9fafdf11cdfbf0280d0d40e6503f708234b8995462106a3cc530ef7fd01ee6a5ffda6891754d6703c4603298af275b4ea92e5d431ff25be87049a9f" },
                { "sl", "563ab6e366611df6d423290079eb3c722c87a433504ca4fdf85c2189a583ab207a07b8e21f3bc7c56ca00bdebd2e475b8561c541193b742a936eff19a117c651" },
                { "son", "755d351ac3c0ed746993f656c32dba90eff7182aa7c51460c5dae1b11b66f75018125837646a05630e58e3de28ef6c451931c8aea99b206404f603c916a18452" },
                { "sq", "04131bbebab6cdafd29ab20c38c7d265f7518358483afe78a36b6db6103a07c590866333cde6e1b8dd854197f0f934c7d41428a2cea35761c80f40cd00318c64" },
                { "sr", "0981f521cc5f253b53ec714aca13b82511ed086105bc253ac8b1ddaf4b0c95cee41e6ad5992a76c8787edc7dfbe4842ccf9fbf87dbc54939ec6ba2cf57de8cd1" },
                { "sv-SE", "43d8392dc21d207a2364ad0f30ec47fc5e984484655ae2386eb3b13038457935f89b587174c52e2fc4bb064a5a0dc9f1f5082f0840f1c38e2cb52991e50177b9" },
                { "szl", "ae291ce502d56b43181ef7bc8b867ab3b71782edbf5a60e191497528a8b77902c614372fced5be6cb2dfa6d61a9dc9b7c14a41391639cc556824c6c8f08e1731" },
                { "ta", "8565989e0cf131740a0cb5f321f079d71d83bac118d0472053123775bc76296513aafb9654804bd887aa7dd0bbae36ed4a9216082fd041b5b46d4155d02a2654" },
                { "te", "dfcd84e98b7137270c8cebc564de00165182d5a0a7598b5ac55670a50443dd2f0c18df18bcd207453d5f6959199c8d693d1698cabf4367c153bc69b53b803047" },
                { "tg", "474d630d285369210aebf0b129e8bb04701861217939bd833bd79019fc95b0f3b18a11306668765972af843b35f6c6cb408f5f2f7a85462731f288305e768e68" },
                { "th", "43a46b4442ad812403c6a12f3ab2ca1b5488a7b19df121cf0bf046863a9708875b7b69760a92f68db9c5b858d5d1b5e070f7014d4b994aaaa8c9ae93478d5c35" },
                { "tl", "063375f28bcdd26140b3ceee4ba50306b3290fcead8afaa5d0ca0836b58fa6a2ed851c8db35e4a7abd2508633086aabc70be8652631fd550a78bb53f81ac825a" },
                { "tr", "a014e90f8751acc30a620ec3f40609264badc041118830dd4d748619d0a900abf1c6c059a464c85fed9cc1b94b3eb56354439a912331580b65bc29ff2137ee33" },
                { "trs", "45ef67a78de133f07c0b0fd033e55fb498594c09f9d65b50532a7595a997d10c90235690e10e7980fd4101ddc31bd84c3b0b18c62c99f317765e73dd27123d0d" },
                { "uk", "4be0d28026a623bf7f6f39ff8329ca4914dbbf89f44f8eb51727cf3602c45c4d3eefeeaa60cbaabc0a530b2c2f76ec6b99735bdc7f9cbf4e98156488c674ef53" },
                { "ur", "a6d0610c60f3f52749067ad068819e72756d8fac8aba6e9ae22140b12477d40984c0883d2c57f0b72cebd087f34721716746a133bfecd9c92a298675d48d357c" },
                { "uz", "3e59bd8c35f0124959e89e7593abda07cf8e35bd491adcff677bf9283dbe48e75accd721dbfe6137530d5ecbef566c79af9df6b140d181c9a33951644017d2ee" },
                { "vi", "a1f555f3a352823171cd4e7ce4291a18dac0c1793d31b870cbfb3e404df4bb323df01691612cb719e92b589e7dd3cb9000f7330b30ea721a44dae7c9248cd9e8" },
                { "xh", "495981dac4c0cee1fd42ff4a0b289f4b35851eee698cc12388105af723cfc766e5f7c28308578ccd633ef01684cf3e286fd36cceb90ef0604920a18169da4803" },
                { "zh-CN", "9285db5e52fc9db5c4cbc91a92b5f68a6d5fea506c4493814c8851dc50d8caf7d1f93b461cab6cf225cb54992ec2ab8f0dc639d5c48a22d3036b4b682d1b5381" },
                { "zh-TW", "7a0a2e5ecaf900dbfe7ace46ab4048072652e44610cd93ff784388d31e3c6aacf002ba6f340a7912e1df3417554dcbb56db813c4cbf38fe8f06279600e0b072a" }
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
        public static string determineNewestVersion()
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
