﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017 - 2025  Dirk Stolle

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
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "136.0b8";


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
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/136.0b8/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "521eaf800ecf6dcc3b1833c538d87a67a646c40b1ac3c907c4feb8a51326df1bbcc04937c030ebe3a4f681e4bb240e195e668a48861f25d36ec917cff560f9c6" },
                { "af", "dff6fdf924ec26678be32e08d9ee626783ea5261c479eb70453e2db96d6e694525a78b79976ee3d834a024430dcc54435f106a1f788c1de71c65a83a9e70c0e6" },
                { "an", "ab4ae258cbdbe7802da21da92bfd2781adf6f2bf1ff2eb4e4ed67a3b7c3c227314021043980d23b9e9fad911b6950ecd1a8db3f88a25f67972c2ca446b9a27e2" },
                { "ar", "f0148de1242131de8cab68608e93d9441ebf2f24762398ef4f26fcb235334ad8dfe1428bd10148f391789c87f6c61e311b80abdc9a17b9f23cc5a9ada6decd97" },
                { "ast", "cae3f5b121be8c84ac9312e29d671b68232a2c9eebf5e95e760ffa9d025f744868ea935f9310f74d6e0e01c0a288ed10b7efb6c4e8f9396954a5d1ba00d05812" },
                { "az", "3b3145ad3218bbd17d1c990bd3b4c3981aa4c051d34f160b7e6bdb7e5b38f84984fecd0a0304a6cc8c837b1e6f6f4da40ec3ecdefb1066e55f8e99ce3601e2ee" },
                { "be", "fb827e8490881206053ec10024e0df346f967d5c81faf5a9a8c35b68045ad7e5fcc3dfb1326bd376bcc9924edff71ee22e84f7ebb7423bebb2685590497a9acf" },
                { "bg", "6fa0136655da2e176f94c4726bfa18cc4b785fb6f3c55e7341e8cbf604c53be42eb6ece0750520f6a93a81aae21265a798a07db8e144e29fb80b6dfa68f06268" },
                { "bn", "468c3f29cd003d05219ac8f39478d66094620566820dde6b4c3ec0da49924c94ad8f8254f1a08dd31b6d99d7cb97cd3338e5bd1ee15a15ae4a2e4b7551b76fce" },
                { "br", "6d66dffd2b0dd3ebf5aa1326934a0b2542a8f3e1f1b38e02776b30745c3b5c95a1895ee4dfc8b1ee5c605013b567e79543ebc1c5df1c178c8808573ff03fc1c2" },
                { "bs", "c8221c9ecc80efc27da50609c59ab75426dea60e85b5519eb4dd37cea785f24309211f38fdc98030c4135645357af869d2c1b16f4fd07aec15f88a03e53c6039" },
                { "ca", "4e13d8106d77918b6bfefc02516ebd3b8fcdeae8b8322b6b9435b4c521ba89be03ca5435fba77977e6850b1413f7bbdb54fcf4e749f4eef3d4f7dc882ac2106f" },
                { "cak", "aa909d821ece9f10e93b634d1eacf9e28dc719a5c663fa38d303c9ca2057813a817f28f17de5786bc553f38d53456ddbb0fd9cb9e25d71ea946990b4a5de34f5" },
                { "cs", "d1cf4ba643407a1ea5e58cefaed4cf68c0ad55238e644a67d1710aa85ed67beea506b4a89032b85636e64aeaf6033d76ddfdec97ec5442a1a7498ddbddcd4b19" },
                { "cy", "cb877831678ed8a6dfb0706d4d111d9d7c5906b012ef23ef19355b91593731c16794160e088ad90731f7bd73a5119bef7ff358737c37a8f569eb2ab2b7b454c8" },
                { "da", "7bb5ae43c8fdfe75bd202fe66cd7894653a2886267864fcfc879273a2acebe53172a76a38125e118d49a9897b45621fb505aa04d49cfb9371ebff4978fb186e7" },
                { "de", "b17ec1e8948686c84649fb211499a1d7ded122050697a66287bc3fe93ed879c37d9b8a6ed17f267fde7d610d4fdb8dd75abec0eccd66b06bd1afa0c50f94ff7f" },
                { "dsb", "ba05aed96556edbe6ce442f1af3262c671522da36e61c2fbbc8fae92894b77a0805d509f77c8071fcc071dd13eda9f0495fc89e0cd0b5a02777be9995541db8a" },
                { "el", "ec1bac736bd826cdccfcd9c7183401ba67769e10a78f4eef11d38624fc3eb2ba3541c7679c8957da087e1127419b3fdf6e8ceb813cfb721fc46986ead370dfbc" },
                { "en-CA", "e96cc09b8dc3a30336650d29f22209d412fe0187cfdc98f561e04b6be70217a73c75cab8a020f0cae0c95355c009e91c851b3dbce3d389992a6f123edc1bed90" },
                { "en-GB", "3d1ad8bbe7b9920009c1d58711c66d03903ed6950de785444a00926c41d6fd26af6607f4821b24e47900350733a0bea0463b2edcf273623d6347643f502ea2e8" },
                { "en-US", "0a9cd44d48e150e6dfb16dbaffe16910ee4df4a57af54f341053bc571408f5610a2a0145ff494f0e95c4395fbeb454dec6130f7947cb813af80bebd5019fd5e6" },
                { "eo", "3a963a6ce74775f9c9008c68d0e0550e560ec0620409e196ee83d539d897e75ddd46f685f6e1e92b5c0d0eac772c6ca4342c0ac89c70b04be5623b77f0f4c4ec" },
                { "es-AR", "0ac338c059ab0c8fad40d85d0e99f2175e9919e5c1df852d65b681873062d3eb022601ae5638d69d413ffaa30f6365165d2d9ef1827dad0163b3e22feab46022" },
                { "es-CL", "9c017e44fd09119ce099ab55e83efc044c99f9dd92c03baa3f73a7926a352ae45d435fb23f51c303b10accb930d4dc393112c429d1a6e1d5e80a8de1bcbb3520" },
                { "es-ES", "cc401da6165839b2faffe6ec0e687427f7a41ca2e7654495bb5785199e0bb9adcde35a54f8d4810d2c0efe93fb621fc4460b4b6636609c37cebe64601e561508" },
                { "es-MX", "2788c2317be1090c018ec7141b01c123acad6e6f1fccfd7b6f19d4f4b7f7c835c1183e5ae640a2b6fe4e4b2c8cec1d7a0bff917e4b6056a08e02da2f0624e01e" },
                { "et", "13d9b6b656dec0241adc909c82c6cfd6ec9e283a3e98dd1ebed2a43dd00adbc9f9876df007c8a4f872ec2cef6c065b3a0486ca0c8b1dbf5ef5a11352fad0e07f" },
                { "eu", "cf6ce17f5ed51b3b458cfa5ceb7482f8bef9ed43f83e7caf0b2c41ed46cf012bc862885bf212623606589b64a4647918ff228279b6bb2267a42cfc79be58affd" },
                { "fa", "8d148194485fb05f1d0151bebb7834e8c6567350ed92be1307a3c66b2973a76f37ab607f94db0573725df04ca677712119ff2f188fae09d7ea636a54804eec44" },
                { "ff", "58c50c56e85b35a046d50e6d25a1f9e2334fe2a08a2d530d9e1376c9143d7cbd7cce1e4f009c95310f3fec78b22e024f48219cabaa70dc1d1f646e7dd0bea508" },
                { "fi", "399342939d81682135167971e0ecd42af44ebee97f7694fbd4ddf4444b1851d510c0a8319f5eef6de16b2fdcaaf06f990faa21b883388a3bcc4e3f8a01c4eac7" },
                { "fr", "3038bdf6719ce69c0647418e06b4f0dff1557b7ad20fd0579728ac2e330d51b96ad22b6b27a80b35650399205f45caa9d866b13cb8687fdc7010de49fe24186f" },
                { "fur", "699104f8b5a1e81f6c373440f012ec68884150285336724e5d9038b4d6fc4ed0c28e788c2ce704c4485d755ab092a924a5be25be74e0aadd21bbf1446f8e7576" },
                { "fy-NL", "ed45b581e08e6bf2e7ae47cabdd6878638d0ab40886b81ac34491824643cdaf33b125049531aca212b519ea8b27932d9c743f1edfbc34516b1757da841e6aa29" },
                { "ga-IE", "33fbef8badd446cd121103e417387862ca1b5b4c8097e8af97d5dcf6764959b113b4b6d90a1740f465c063edfa6cf3edace05361e2c0cfd7f8a47fa3b876bef3" },
                { "gd", "c2db319279567450e539226a1c9ca1873ecddec1d5951e872862dbd8cdd94e974820e40fbd385540f86c259757fd0ceed97895ad87d4ad6b6fc6fadcdaaaca2f" },
                { "gl", "ebd80d9f357490da320e17e1d79b5bd93345e31395ed26b9fe28e2a9c35218e6b50303c8ce907dfabfe5394deeae20be497c5ecbb7818aea098593c4e8a8e93e" },
                { "gn", "26b958887804e3f441b858c9a188ccf6de5456db5cf927b83438062f568b61c6d6517a9dd1d5ebbb72f83d42140947bbb684e6683085461018d9c76025f5e8de" },
                { "gu-IN", "221fadee2e2eba372e225ae5549e38c6512c9537926269d6575d384fd8ea333ca2d44e1d67d6ec3f98f5c9b6e9fc558b6248699d532e6e09f3af87c946c4f32d" },
                { "he", "7ff9374766076110be6d0b0fcdde137dbdc6e5d16a895d9e6232276e7aa9342fb6f17e2d3dbceafaa6788f9e170d977b5b018fc6ae67939bab5dce4742695fb7" },
                { "hi-IN", "73e48f1bb15ce73e39218b8456cadd964ee91d3a91c16c62e49b8d861d700d014367cda73015b66723743d48f203830ebff4398ca03a49c6f6652e996e770a07" },
                { "hr", "7a5ae556b187aeffd44c03feaffd9fe53059b2265c570c602d7374218a0d59ff2444f3b03034b7805f8a5524746f1896511f6ab59db87a888e84c9e96168670f" },
                { "hsb", "26494d625375febc98bd6e5121b89950cdcc7248a1269c965e825d21b88bd6ae1b2a9b162d1de3767c6420239be5a85a1932fbaaac1fc975d943f935c71d390f" },
                { "hu", "4f0d2fa25c3776feef8fddf0ac6ecf20db5361e0e750bb52f42b70226b2e197cd03d5cc9b44eb514322b6410c2e2bff53542c8ada454f1af669dc810585e2cf4" },
                { "hy-AM", "bcc9e47cc23c017cd69de697384591b7a41f41e9c72da8bb6a2ecad1609b3f9975440111a1df707f7d8706f787c3ba7c813766a52cd274dcbc64539d0498a2e0" },
                { "ia", "f7c456313e925abe70afb1943ccb4194ab143ef1e4f31b48a72f75ad9705e27c619ccbba5ffe35924abf23eb8f20107aa4ca807fd120bec02b9452b1e97bb207" },
                { "id", "62a1489757db24fd24622d0a17ae1c98b0a134b5ad9484357947b6ea475bfd4ae9fa35a75cd66a46087a8b5520e9b28b37019157787e8199e5e37b6065e750ae" },
                { "is", "7f0c4863d993f837efe42c4325d2e0619407a59e3dd743ecfd71a01ff272496a16113a8bb4d7216f395de5064fed6f90911be0724ddef8398aa28ca70aaebafd" },
                { "it", "0d501514f2cc73c1ebce76a694226c81df5df9a4f4dc2fdaaa22877163c514b2b8344b7a2cc9858d7a2bc59f4b114d90ba6dffa569d74130370183f53a814ad7" },
                { "ja", "9cab5c4addbd82462766c65171c20391b7f24d455e38e60f779beb9521cc208f5714f50bf8349c63ee0c481c0dca1e2b245ee2e9cad9e6db33aa5e1679ee4a0b" },
                { "ka", "61495570cf8f5df585cc98c12d57f5cae1a3c4475d9fa88b16f8fae531aacc9ab92a06a83e0e536bf81da8da8df61866c0eb58a2658a380296d64e90038ac03e" },
                { "kab", "f203cfb077847bffe24ae114775e2789797a16015979384816fe4ab009e99c5590aeb7bf7d7e3d369592855c6895787cfad7f0caf63f8ef8119b243aeb00d86a" },
                { "kk", "642090b128d60db40b15ae8a63d8dfba8451e0a0613d80f4634cb6a1053410bd5fabb9d2d90600ee1dac2116758515c7038ef90ae4ace313d0ef395448035181" },
                { "km", "20cce80a9605e56c5590554ba8e4cf318262f999366e48f1e5540048e0b9a39fc151ffe2286b8c43da3f36d73fc6fecfad8198f76543502ced96234decff0ac4" },
                { "kn", "687e4c2e9439b02e1575c07006d8fc7034a237ed1e8ab66cdbb222e50f444bf24b4fb85e10ccd0a9acc624a8f8b67889757613cc4ce968caf789e10aef2caf40" },
                { "ko", "9692f3996055900fcdd4bf82825b5339db4c6d427be9d7ae569676302702603e31ddc0566328b6df1d16acd5d117091ba3abd6ca6efbfe4715b0a0e1a28663e1" },
                { "lij", "4a78a2deb511594c77ad8312d92ddf602b4b17a9217afa3c332eb3070946fd669d8f2489e7ee359b9515d23a82498ce2ad2c5b5da3983a04dffa1ad8308144d5" },
                { "lt", "081f54bddf09d0a9fb96ef54f76276d915ff9a1c739dd58cf1c1b1e7823eabe2407b9058b99d866fa33635139306ac527be537cc9e4c9c17e490b68234c57a53" },
                { "lv", "28d4ecd7663242a0dedde5a8265e382e4500bf65c103a9a27f2f46ae98b2faf83a93e3b25dc49b9fa1e0e64a99b18684a70a110dedf3d9ca9aedb63049b40e60" },
                { "mk", "581942ce5ffab851048c910c820ed6bb1934687907e866600a6f8fbc5da6fc7c3e02a8fceba454619136da69367fcfea16f656e81dfe7d29475b0ca633652ef0" },
                { "mr", "bd63bc1821dfde412833731e6a3f1327da40b40147b3a1398b1f57ec64ef0d171b3edc41c3663d3ddc26dcf297c3698e52ea5ac33aeb8892381cde041b57c25f" },
                { "ms", "9d420e7d2872aadc3d3a3178b59299c166aa81932574f68e48003935343e8d7a1674619a7a0ca1990c10d211c74fdfcec30064d03e71134804d5aa683f9ea948" },
                { "my", "477206d180b77f7eb4e16627f98205bd99c55b87cded2fefa2d5f4929cbd093955f92098f0e070983d07dade3495a2ba3b92ca98c7b8fde085bab6c258109e89" },
                { "nb-NO", "2a3ce48738ff2fb19bdd7bcb2b9bc57b8d0574bad94a1e40a667cbff23a6b4e5732f5bfbe824f836c837f4d9cf7cdb0bfa536920e661c0a819229976b186aecd" },
                { "ne-NP", "5ad0619f5040eae6d2eb69b393df7cdfaff7b0ed7f11aca58c4d32c6fa5d6fbc1ffe8bbd14e4a930f69ac8d6b009bb6522cb81b9a8ef30aca7b346ad5e53c1d9" },
                { "nl", "ae92d8d6d2c1c8efa887ef347e4713e8973e89b34476bcb0321b796bfe7ec56b1e2f18d96bfd0c945878c615cf43bd38df58574b5b591bf283163707c5cf0d25" },
                { "nn-NO", "d348ca50d29b643ae7b083e78c4c0130f6a347d7026f88383af96eef8adc7d5c2dd6ae50adb1c6996af21cff7eeeaedb928ac437424610a7a3b926544959e060" },
                { "oc", "f9373ccc60c45cb69902ef8c2b435293d4b5ebc97a4e82cfd776065a162b18473fec420f2e0487f3b17f947b02c217d6d07cc28fd59dfd0cf3355a68e5b31687" },
                { "pa-IN", "4053a4eb9b6494ca88713b8e0d4f8411bd806a1bcbb23162c3bf3c597646ede80ae4fb6476c1182aa62e63c8d6d0fee66c1c8a21ea7c8879a6a00c1d6d147034" },
                { "pl", "76b609f21120aa970512afa0c7f85043f7f06cae134b76fcdccaa7ced493a3d245e9a3a136a0c032bbd748cb56f7c9ffb87ffeaeb5f17fb29d0c7ac9a40897d4" },
                { "pt-BR", "a65c952e21bb420b80fb3a158b45a6ea178981f9aa483eb6fd7305a52ae519ff25f38f5f5ece7c5bf7cc2229523515c4fe0478bfe4ee57610b2f6b0c49fe91cd" },
                { "pt-PT", "bea61220c1bdbd4c56e9932985e88c873a662bb77bb7f0b59b9db1e5a9e593f328c43b22040f5c2783210ca569cf239f7024c131f4aaf13e156ac9847d2e0a72" },
                { "rm", "54a27edfd42d947548e5fcd7bc8612f9886d271fe91476f6d9d6022f3112000fbecc5ae4277c0329435e96e095278d94f8b8c6f15b01a59e60ec3c3ed424ab66" },
                { "ro", "d367307aee9f48e817a1a569f7bd390ace337077d82c94b6d88dc18084756beb28d8e4e222a54c15132a08bf4b987fe512b15d558d1da0f1e99a0e62e9fc7709" },
                { "ru", "b0c2babca788a34756d3c0f5c72625c04a340bd168b82b30f4ced2fb06d5dab294f711f31a3bed88f5a66e99fe32c4c354db8cc9980d94cfb57da4d8840a1634" },
                { "sat", "134898bfa1bf83185d2dcb752c4b1f4a120e7a29c96bb9873af93a49443e9e09078693f35bb6d305277a097dbf6c886f86c8b482264d5b784ef146686f779670" },
                { "sc", "1cfb7aa2f4a3b06280d44dc92642e31e32c84a768851042d951e58bf4ab1767dc0c2b57711a50339eb00d17abb351a93b7564745fdec87b056342cd4b94fae8b" },
                { "sco", "261120e2b16a5d0b320198b7be765148aa46b9b713da10715d37cc1630c3316edb8bc983a8821e00e1b4cc473a68c534aefbb6c081530af56eeddf29fe74b067" },
                { "si", "0f754ec22f83f3597aaff8ab4dad18eb0b19c54a111dbea0eb9bd23cf55ebbd36f2163a3cf6855b5bb181ce0fee290c97d35c73f197af8da269d07325d9a45ca" },
                { "sk", "987864f6b48aab2e5ac730a61061d1e0e9452fb7e14a4809e0da9e8d50168869a9ca0af7ce3f3a6f690649d1de952dad4431a39ee2d0432f2b0755dac64f3667" },
                { "skr", "f88c0a532e190b5e3dc16ecf33828c69a681e4318aaf4b3a0249624dc9854fefa2dd55e94e341637c10d0db00c58c6111dd746d15152b1dfb8b7cbdc4e13073b" },
                { "sl", "d3bc772b90b3031b3133c3d8c55233c7251d03c6624f491ad50f4446e43e3969b59eecd3054c29046e49617e84a7244b5ef4afec8145ad1b54f161696ce0c0ca" },
                { "son", "3e65b22d192a883f6d9c29130900e48112e1472056458f1b9377b930e7f283c7c1a0ce99e40b6f20c6e546ea7eef4bfa0dc5ef6dfb036c9c1e450ea1978b301e" },
                { "sq", "0511413c231ef314339ea2dee424fe93b656141c77abc5196631c1a55241f113eff8a7217dc4d283c36c1c009640d1fc3cd4c05a1a49535b83869898c7eef8d3" },
                { "sr", "7734e6b4217bc3ce8430d17f6bec0189dd262e21a263b00f6412e256f437c3523dcd82110a02934b55f31a7afe3a1c04b9c35afb5dfd878ca47d25b7e947439e" },
                { "sv-SE", "e4a6d982272e4c4fff98b922d4f0fd9fd40621e38f2f3e026992ae8bb82f894231474f69343d8b930ecaeded7f37c088fc84a715ef721c209bda1438ddb90427" },
                { "szl", "a7db742291de06f9bb191c3497d38b55dece93fb16a7ba266ec1c0785c1c878f16f6b0b8db643b9f98ed59d4db568f5ce7c72e45daa0c35d373b5f5b78fd7131" },
                { "ta", "d6a7b3416b7fa7a9f4d043c7a5a0b7ddde4d20f25047eb4f77e2abde7ef23cbe1c0857a5ca8299826c4bbe1349332f4cae3f3d3d476dc63088a595ba03aa4e97" },
                { "te", "6879ebb65efdc103a5dd2087482f9f4989381591f187e74beaa4a862e9e8bb427f14f0d5527b25bdeae8e825e47f63e8b7b41b1335505acbdfb1341c2b308e43" },
                { "tg", "e58a757ac7baa5feeb193cb6409a96437aa49b7ea94f3f3cb7d365311581a063ac538bdfdde60ebbb6dcd25fdcf2a7c4be11bcd4fbb68470d0d3c953fe09a9c4" },
                { "th", "d147d233e2cccf2aa5f00b26ae3ae33bf5564be55966ccf4edf61bb4b46991d4939d8cbe9fe67384b35fb7cc39c9fa10518d8e46a8327eac80c1e34ecabf62d5" },
                { "tl", "e67ee06467a84829e5a8c669702a6a73a1f052816b75b0cdd220aae929832a195a811fbc9e30b84a1ec61c3b1389763fee59e3640339007d923e0bcba2aabb53" },
                { "tr", "a91b71d4c7232a8b0b136c31ecc067f3abe8a87163aedc2441ea564abf523223c1769f7f7b1f13f64ec26efb19c184c4e44749d4a985f0076cd3e0f50e33522e" },
                { "trs", "f47c8adac55b6fd9c4b7ab1609ff2b88c099241c3607a3738911209b147966712de0ede310b1be0bc25e2be4f5e248a0e6e9b0fdff3c2a14958c3f3be2aa4fc1" },
                { "uk", "de87ca29cd962c347eda77b931fb127585963c161037cda70cab55cba80397815bf33cb63a485b0876ddab9ee468c8e3eabf4e98d47ae1f6d76c0a7e1ce3255e" },
                { "ur", "8fc86dd2f53cff1dcbe227a3d0613246a4d2e9a841b6af69fb129175594e7ed1d33d3dbea69132f2880f6e34146a6ccee5b49bb9c9ea72ed2363de0a2df87d00" },
                { "uz", "ede1a5897f017545180d942cd4f7e01d4bc593822e6c6babb1cb4e6c40746cfaabb4849704011262a3e85885837860b29161351440ddd3a720a4cd4676c8c4ae" },
                { "vi", "d95d558418a43a410c1e856a266da0c8fd9797ec15f43bdd2d615e3cd6e45f114bacbd143d2dbc4554132c1cb0bb4b29b8d4ee8c2978d42750565d225e0b32f9" },
                { "xh", "b3d620c8c05b1fae12c520c969fd5239add2c61adaa458cfbefdec67a217f2bb696f3f4c724a9bec787dd4296da8ee747b2f9ac004324b94a180f85a45567498" },
                { "zh-CN", "cd01f0b7a91b9c12716187a354ecd8d4e36539da417657e65d8cd17b21c1753b81214502ad1d6f912e7a434d5b256f45067f378437551947ed44c56d5bfea90a" },
                { "zh-TW", "8e22cfeb3cc1b191a9abf1555fe84b1b2d2543dbb3c8281222a7f2069c111d59509e6c46b8779966901ce2a86859a6a682f551d763d9e48054b507c2e2da7924" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/136.0b8/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "203097c913b3ab74b10fec0c7d8b364c812cde9c2f57061678db20bb34a60461c013217cdceb5dafde7b95ec29d66c6517ba81074d3ef379c80af1903c709046" },
                { "af", "5d54c58bd78fa511b26463e315f3ac823f5ece1e237c2ad5fc3e771345476ff6ff75fc600c06f228a2d53918f132e0ee0146d11b4c2a28276af81e86b01cdcdf" },
                { "an", "9d281188c71e2a0783c26d7276b74566f921efbf4cb77989efd6cee6e612629b7d66583eaa0015a61b59fadd19ec6253ad60be11cf87e57f63232c8121316305" },
                { "ar", "6ab57b24c18d0bf708085c7bd01cbcc00dc83ea03452ed6796e773161897710f4f019c6b3506a997ca347e100fd77a199284f996ac5734dcbf9c16a2e887d06e" },
                { "ast", "4e7c811d452dabc736bd7d439da7b9d412b27f5d8b1b9de43c4640e48aeba9c9631a1346a0b788d497cfa37c841b09d8acf5446a2bea8533ea3c663a027b347a" },
                { "az", "6bee6882acef1fe9cacf72eab622903cdc73c720f535d141469f0842fb363ed435eccdd1ce995c089550f73f2a0d540a0d43fc0f98625ae338b3585272c2be05" },
                { "be", "bcaeaef30b5ee88f90fc4584becd3613c2bcf41d7024f5d32814b57f22bed1656a3074b23a4aa15bfd8e8233f3e617668780a8863415c81d46c61c9b9b15412c" },
                { "bg", "74e586d66f246b482ce8d5da861361eafbd8424e5857a72e7efc23d16816b5c13f41fc72cf9b8d5601fc3e3b0235c191479b0a95fc5834e3821be356c79c29f8" },
                { "bn", "f7d7178ce55ed0a4f030a80fa51172e282c5bf46f9652a13d11b5ec29f48f6753cfdb7be031d74a138e642970d43a4b613ade07b45123016c477a7c0373dcf16" },
                { "br", "4d8a2bd38b955f64511cda98d76e534855e97c65c7945e9b215e8ce326cd06f1de923e743aa2b46a3cfc261b5afef32f2912954375e1e2d0072b6595948735d4" },
                { "bs", "c1bb184cd622dbb447154312b7f8b55f1f6a9665bd4c3015b96d6d99b22d57ec4986ce19501f9f9ad08bee4d653e3ad09129990b23c55d745008646d9c0d3b34" },
                { "ca", "4112df65dfe6ebdb924fa983e77160af6ea89d483a517e7710f141adbc7124e4b13abc450ace59c4827bcfb426eef7090f824265774bf1dc0a38d0946c5a64a9" },
                { "cak", "34217320e2e34405e7e2f3e53db173287afc0989805f161be76c4d444301901558083ff5567fb0b6663897b45231e11561df966e21a96c411e301a647350366b" },
                { "cs", "0d60c8d01fc8406034bdd6435ba40d4e673ed03836669f8531cfe122ae5dd383ef39b2691a1c456bdfdeffbdce300d38136d7f062209e94a3d982b67dd0afc37" },
                { "cy", "f95eb98ae8b830eea8e8adb141fb3ca79dba3bf1e7eedcaa606612caff0a65a42b5b98c8febd6e28953d339f1762849c0392f159a4a21bff002bcb1fc63414bc" },
                { "da", "ee5c9cf8ef9cea88133422000bc113cce87d2d28460f8f2a91daa514de553773c2b9b0bc290ca3dc73727d2b046cd529338daca474d99862039cbab9aec023c9" },
                { "de", "d86d51bccacf6a6cc3913f10707c4c228b477effdb4fc0775da48186e5214b950f8d1f5e674e7678ebf6a214c3a8e795c83530769bff96c82f3dd1d60be2fbf2" },
                { "dsb", "fed55fc09abefd6e6923ff2da23855dd2bb584a634d096ac37c76ff50b76986dbf21181ab147b4a9689b6398f77e948055895eae3ecc910581b9d7395ac4c6b2" },
                { "el", "026b44ca9e250d27ef5d2304d63844840c58ba308f756c6094f76441765e6d646219b44ab9815f67ec84dfbc8b3d45e4383f24a1bdb706c4aac1696cadd3cfaf" },
                { "en-CA", "d65b184bcefca5b66fb6619360f68e37c61966953a0a7859d8af764e1a8e870d8e8c4cce1b4cfc29083e531e20754b867236a88f6d0e015e6c999928323d7702" },
                { "en-GB", "85f61efd0a13ce7c23ba4ac82cd6f3f26446903e4cb349529b3cae473c7019b102f46c47a52dfcff1ce3ace3bbd6fbc673bfe31b012e7a46ec92d6f67a240d84" },
                { "en-US", "63e69c8b01aee067e7fe2705751d6f5c63f8d17f024785ec78b5f1e5c87602faaa3957156c7cf62d5708c5378822d343486052f782cecf0f3ad7e175e415be1d" },
                { "eo", "d80e01081e934dca44e67b335b7da05d8f75e614a836af7c4af88be0c984e311b27f705731cb157a2cca07929b2e7c090d8d0a872cb28b75b13012c6334bc941" },
                { "es-AR", "bf00136476df30c3fa97d7fe6e3445bbb510448ae10b5c047ee8d2e9fa9f639c037512b74edde489a8c869270bb8b77f64c4d7ecc5606cfd12164417c786521f" },
                { "es-CL", "37fd590601a11f300bd6f98a10a9e0ad5fd6b44a736edfefe69ceb5705fad460cbcf62ba324ab11b181e92b6de83fc9a4cc8bb7209054961b8735399190447bd" },
                { "es-ES", "57d68af1d7edad3770bd25debc2d6a14eb8ecd7c73f17f6e9903089fcce41be273bbc5e060c93048ca986277fe9208266f588a2ab9304e043cb279425edbc7f9" },
                { "es-MX", "97e8ea7662d794778e82503b29c6524bc709367dd48533b8614c412791301445ca84eeca23bd51c91456ac07c7c6abbec937a7f90067ad7128dd2daa2c3ed93f" },
                { "et", "4303b175712ea8a589f3e561e6e789e82381e46c74c042e75c067273fd20832de0a1439c83d8468dd17b674a3420f47de065c26d2bb1f88d33a20abd4c1c533a" },
                { "eu", "9d44d97779ca34b1a5f473bb823843e6b679d6d2ee25884e41cb6f2797f1b122a03cae9c2239fa900628d5d85e80c9740c883ff92c98ee6ec22f2b582950d1c1" },
                { "fa", "e520c473b57b961dfee4196ddf61bbbd35b64df5da1430f4a9ae3ad8282b469bf38cd8028331dc4542c95d9676fcd05c3d658ecf56dad638555fdfeebffb5a25" },
                { "ff", "2e65027f462ca65f7ac7e37ae8ecd20bae8697d0930f5ca81f077a6bb8074b39c118a9dc0e46b7ac2d75a23050ffbff45440d81f2958a6810bd4d410e425bdb9" },
                { "fi", "bb7864aeb3acb83864310da0550e50031bff59798cfb719c9833f3996e6e7d1cb35ab1506268d81ac0435d75fbdd5bb6037d7d7b648dce96bc9f465988da61c4" },
                { "fr", "2b44e177c789a1f7112972232cb3a38ddf0829f65587320d3e40ecf2305d6fd5a8ee54fa5f1f8795528d041ab688bce1cd14b31b70008b9e8e9fe36844d682ff" },
                { "fur", "87f8511313a38c57dd91a126ab7fda13cf36229778a6f0b777178acb6dfa30f9f68db0e9e53a91f3d7e5565d1bf5e046f70b69da162320d9971e04e564b61ff6" },
                { "fy-NL", "99a1a5f9b435236ef95d747926f582e93e75bf50be0e07938f0bea20d0e0dca16a88d98525ba0275ecde962ebe2261deb3fd888956f5c62789252db7b93a6258" },
                { "ga-IE", "09defb37f1620e8777ae2046bbde88f4155abf911bb948cb1186afade91cb8f92ec276b6bf497f4ead8e47febfa12757f2fccd67505ac36fa4812c92893887c5" },
                { "gd", "76ed14e6421b2d1185349fedee148759d2ba3f55ffa638e6d2231e4c932d6cef114380612c0aab9c7899bf1179237117f167ac48a483eda47bd38239a794bc2c" },
                { "gl", "81ecc157e212044923f5c165574a32d987cabfefb38a254ca8de90eac2c77d015c025cfc457c8aa0d0cb3aba215c8fd2caa8fb52a5692def0688735136cdd495" },
                { "gn", "7ea37a62f492a226b6e2c4c2f7829bff2597d348721542d0c0d5beb5945cfa1747acda72e3182e057e02be7138d888a66f9f1e908504e653b0893f665248c26d" },
                { "gu-IN", "c10aa3e4a535d4d6c082d84483af039298f3f631831adcd0b60d4732e225800ca90b9891092450c851c98ae2767c09fdaf994def828cf4280acd11db476fcc5d" },
                { "he", "b9aa7f993dfba4e1794bcaa9999c1dc5190a02a519706bb86aa08a79e134e6bf11e80276cba0508fbec0448ddbda75f5af52bb0273472f9ef28a7709afd113fd" },
                { "hi-IN", "53c8b1382fdca2244426aca95e45aebd5ae239d219572e84729f51649f6cf3a1df84218888fb78e74b4b78c9e69670f5ab779b51528def5f0ebdd4e54aa0de7f" },
                { "hr", "7a924a0339ecd0d995e3bd805b987cbf0a9a4801f9a040cf39386036dc70876ca96fd71d25b47d1b4283c1c165637b59f74f5418fba4097edc37aa33556a2f26" },
                { "hsb", "f8b5e7c3288d43e7299a98bcdc3340045680c5b5e0f65bdfa577cbfa3601bee617894e83fd570deca7dbae38c08e37ee8b83aa4e7f724fccbc60b967ba1ff81d" },
                { "hu", "9e545afa0a0e18c418f713db33c48f7255f28672c1a3f1b1fd1f71c78920c10199f5bfc0139ab1826beacef14cc4dcc662584ee9de7a7842e6dc5d68814c8d51" },
                { "hy-AM", "7ad95eb94996ed80fd4608dffd979c59055a63ad91d5bd9c7c22e74da7dbdcbe28ab0cc951f3e03339173eb50ed809eb833c19ad5b03e9aae7a8b4f852000ec7" },
                { "ia", "583c5923e8a7fedb66a5d2a818a61f86d56548e7e4c1bc6330fb2c25027d9b2520395400358f6134952beadfb6a81ee6f08b68654b4d8ecb9d7f79abbb13a9ac" },
                { "id", "982ad02a55a46964b029cb1a5877c8197f9a5c0a796b88d8a1cd638f965b5998fe38e71f1f3d05fb2d74b8153a96fc29b77aad5389012052bdaad7441b4e5d81" },
                { "is", "fad1962904a66a9d649ebf8f34ee6d67642673b7c9e364988433f579dfd4729f4820361baa5fba29c7d3c0c4b451797d6212a7b504596e9f2f06a1e42e11d0b1" },
                { "it", "8d3c78002258dbad1b97e54dfb843a086c7389941c057e682922220acacd36c3a4e2ba891d9436e08f5298c43b1d0fe4b89552aa6603d7acccd5215d3b74dc42" },
                { "ja", "c0eee0a244f024a600213ad8a6ae72db1fbfcda892c6e115d794a735a0923ab606526f119a2829d29e0f3d2ed9ec4412adeeba2f65a72b77a933f5dc4f3b9f7a" },
                { "ka", "5029530a0fbb0ff4ece88bff62f0fbec2b219ff34a73748b2af9d0bbebad31ea6ec1f3f29a0a4c0dd66d3e49ed048ac46b271e77dad289ff61833cb04fbac8cd" },
                { "kab", "fd396a01795fe06f946ea112f97b0058162db70ee825f5430dce0638f42119d62f46c513a160b785e72a0aa79409cc119bcd96fb2967cfa0647dc6a24e5a46d6" },
                { "kk", "4f2b8b6a87c9a85a8d80517607848d5b45dd94abb58f3e96d7d92d86311b02f2a3a767eb565ec7d49899f2ee2bf37006f31cbba60b3eaaefeb66edb9ba6bfc4a" },
                { "km", "85c4f5b074a723aa017bff2ace723774460011ec9c9f507c994f70e3e5db0e1d652840ef58b6b8f134c813a0a3d66b2fa69891490c4a4563343477c74f03f16f" },
                { "kn", "62b2a7dc5114533a7e176d7465fe2beda70ae42be70eb4e37836088e3380bac3aec76590fee101923e5c22e0bf55a52e40f2382219764caa700e6287ecb73f82" },
                { "ko", "28cd8c33d977031f5236e11bd3d9038969b37694fc26fdb5cb6bcef618f9fb27fb3f56d96eeb9239da233c0030a63639050fd72dd8b1672dfafaf639bd61b271" },
                { "lij", "b2972622aaaaf44e429426498429ae821cf3db5ab3d522c67078700043f23e4174066edba6f0ce884e2e8ce447a51dfd9d50c36065e63c16edd82652118ea1b9" },
                { "lt", "52b607e165558ab8a49e3aa2f1d5969263ef6e92dc39697655abfc7778b5599102f27bd0767505e43a0609e21e38d43582b2d7a6d2be6ddc8046ceadae20d933" },
                { "lv", "129f295e03826eb8f4bf8978124948bee1f6c9cd0c035801446253682f9d035ad77764fdaba29f054e83ccc6f8e2a2f2b545449c4f267367fba17b44b6d8abff" },
                { "mk", "4c36ee32dc6599dd7a4973cd0f940a452d0fc4a522e092070b6c8e2073e5d9c6222f3510b8779a5b6c76f7af609ba0a54b7257ffbbc6e51a2403e659455d2c20" },
                { "mr", "d14582117418e26edb2c18166a0151be125ec15cdfe3a64bab6ab31f7eb15505064d8d924b137adc06119dbd6e8fb4260b4fdc8044d826c8d847864dc01787ff" },
                { "ms", "bb99e2de91753cdf868a3ccfc6a39b34860d8794f72d1d5f8c6f58481089f0e53e7cfb52be22a78747fdcb50a717852b0579d228646585a4f0988cf9d8c78c94" },
                { "my", "b5b1f0f605a4328acbb57962cd9a58ad241052c0e67040113b258d43205e8044d1d00d884629320f72c2db2d62e4fa5b6e7a1784ada8a72a7001ea710698eb1a" },
                { "nb-NO", "99f9fbeeaedb2f86da869385e853dc79a8c45278fcdc06759b0a9ffe49b2b74c84ef590e3ffeec7e6ef95aab5089848cce0b10f1fbe124bca957b7235d67223b" },
                { "ne-NP", "0bd91d15c0e5c7ae0bb93dcfa878502d9ebdd102b668faad173bd0b5d704c8b0a42ff20c67a00e04ce3ee418152f434bbd0ae41101713853369c55ce938e3469" },
                { "nl", "59552b6268aa5347e2c97fc3e913e6aaaf482310a6a861fc7f9c9a5afb0a852fa6bd687566167ee5fac928335040771e0a5a4c3e5e55540044c04bd3ec21e2c3" },
                { "nn-NO", "46c5a337dd145dd6e605e80e2716ef5be61a52edbf142d2a8e355298170fc1bd8d57715e892cabd1ea8909d0f6e63665704c12dd5eff1dc9777813edf965f6c3" },
                { "oc", "14a3bbf200aee344e08dcf62d3af6fe0f29f5ea5fb666555e43911cc1bd5b6ef7420d41fbafe85e7ac038d7ad40bc0cdbc2c16cb32862b19b21e4298099b7d05" },
                { "pa-IN", "8bc4bbf1fd3397c6a53f31e87960269eb20b80d38160da6d1085759e2258f0a5856de900f631d50be109da389472384bee1e76ccbafdcc2094d90619b7e87827" },
                { "pl", "4a4dea2bfe04d0690534008064f4bf43fd757ab2ca3c1ea9299457817aa1a49a772ae64d5b4b5c196c302db9f523c2bed284a4a6e1d96276a1052979a72f36b8" },
                { "pt-BR", "3fd9f1f748bf51b22b67e16a5e09873d9aefc93f98075ca928d9820e41b8adecab8ea42535d7ed57715129814032de45da39596a0f965f60e64f064984727beb" },
                { "pt-PT", "3f70a6e79ea6911f374ea640e3decb6e30a17040f8392ec4403a9c3431c082fe47e8fcfd212630cc526523e4fc385e62da4175fc0f5701d4ea5b3c68ee16c86b" },
                { "rm", "5ab5a96d5128f796aeb8231b5a15296dc59f323ee8921e437d7b076998d20b492b641546ace305950d840be9b87490fb77d3a57b490a5e5bfb63765e33a8b916" },
                { "ro", "c2d01120e5c8af5d17ac24e42de5f0f49436ab54197e505399c555b82224fab6c4268753c7188399bea61f529459a5728bb1376b51eaa105117221a891614e37" },
                { "ru", "503e0780dcc76144e076670c590a6d45e95325a5830b631f5b9e5f608d887f9c4e4b4444432cfe0f598285497fac17f030c849d4b7170e04712ae9f47ec8985c" },
                { "sat", "301abf085061ffba4ba94606fe29c0f8ec09354be05181444daf20b166cdc29ad3afcbd7f55318d05638fc5642a20b4c326cde4bba2ea39ab03f0caac630729e" },
                { "sc", "96300cb5bd638db078246a5897bd154270488e5297842c26b482f1c123134e0ef6e57b5d7950674869f773bb04b42d42c97c593f640b8f7a5dde648075a38814" },
                { "sco", "e8cc96c24b4f028e4b25265a33c36182b6755fa9c1509aa4abf1b0502523ff3d859d1562c419428a870714051e29891c04c304386bdbad8a52f99f3c07752d35" },
                { "si", "d282f849430c30139c524c41d1b789e869b1650df479080950faa457eea88370ded7b460dcff834ec9e25ac7d64f015f5f894c9264fbe43bfef71b1dd71423b0" },
                { "sk", "7c94975193e4b9cb539c65f1da1b6d9e8f1873568a3f62ba179f3937d3baf98f28b36da5aaf5321b8543472aa3cbc3909ed3be877e2d5107d817d3ae361d3a7e" },
                { "skr", "def58e1cb6a12c0bfbcf98a06e7fa43e1b52aad9d3180d40948297299a8a95c2e8e3afea81d7fae54713b7bc6ec08b479b8073e56e0c9e01f909dd0bb695e7b5" },
                { "sl", "0c76b0de47d9250fb8427843c55d40cf9097ad467408fcc218648f1c8018d5453ad59bbc75654cde54b1e8a81f52911f0acef76e95079732e744130cdd2672ac" },
                { "son", "6f1f93b3b6bb7e0cf81fe3111a9ba5c4da53a17d9dc7a0b6c9942b8ad0d6d2b1a395a1f73ef45b4124f2e1a10c8639f08ebc2034317dea7db10cdcfe5df07d6e" },
                { "sq", "5b0fdadb06bfa6bc01c2209c16971cd6725e0146665a355372d292c2715632677a1919b14975024b7ed9045e26af2c16bafc646d76bf28982ecf9a6d54b9b1f9" },
                { "sr", "33c349711c3fa6aec3ebccf42bececa3414aaae81dc6d2c3f639fc67b5f08858201a651dadee065333df8b55019b2090f7ca584e277fb742afd3735be59e8eb2" },
                { "sv-SE", "2816096082d477543947179474ef781be1b6c9e4f88d01d59dc24712c9db9702ff9912813b5990a11f79649580c494520aca9b2298b9e1d7e57192153b39fc21" },
                { "szl", "353cebcf13b4066697b7b334f1382b537c16bddda4500388580cbe472e1f032529e38c533da766bb63115ff295348a41182cfff2e752b68c95b85be13d1e28a4" },
                { "ta", "f389a9b7b4619dfc4fec765f55c7e845bb8629e45072393287eaf2c205a2df627c0662f79889ac9c4057f88b2ee4c4e64987c13a43103ec190c08ab3ef1f819f" },
                { "te", "41ac82cef17f64a1e06aa3ae0e940fc97fa2a5e056bd05f366a0293a073db14f26836c0d8778f824c73d2cdec1577dd5b2779c26475a50fa3cac86956ddde7b8" },
                { "tg", "bb740d89240500eb6c857bfa40a2188ba8dcda1db1f90dd973ed9a22aa71963860aa664a3901778780f969b957475d42e2990165fa989712ac0eeb251e40694d" },
                { "th", "024f1a6c4e26882d28184f9fd826b020e0e9185f30d3fa42d105a9f37ae7f9bd91b6eeed2a97a2e36cfd5f6d7cf37a650aa161647362974bcafbe01be7994abd" },
                { "tl", "b1ca482442c4a32e5e04d43f74ac97d05e0d35dc84f24857e359a2a17361c8e02ac95bec3cd986b1a924166f426257666f10a20fdb27cd9fdb45a684f6fc6ae4" },
                { "tr", "9fe667b583a3b35d9188142cd3122a96fc95c7033fc09f85f279acb48c19426472b33ac481990977f57d13445029e2601d91fd5be71ae0ec7af212603c5b94ce" },
                { "trs", "306d88af62f3b520df85ed0fbfea0b070a5027bb633dc63f2c72b91ec012720e4352b61c2270aa3f0a30ffe9ac00a3911bf74e41ad244eea991b31dbcd159d0b" },
                { "uk", "881ef453c306eaf2599cfa288c09bfb174d16eeb7c1d8d8255e8d9779caed5a9d17373ea7881e6d7a4a7e811aa1caf312b0ef721d0c3e3621845f562845532a8" },
                { "ur", "897256def7696b8cb9b2acc81e835b392ad89058c27712849e32109eeaec62232bff0573c0628a69f6c2a1b29f6841f4d2f91934d775655151a167c0ae1b5c33" },
                { "uz", "496adb638df86c08a00a13babaf45c54be6b6b2257b9510c5538e6be162f45e831e3a751aaef8982fe63a8991907172eebdf74629553a8ba72d91e1c5a50cc9c" },
                { "vi", "6f67d73f7011c6b892bf8c94dbb2ead6cd0753b1a93d45aa121ad51b23a1d35fb9e76abc677baeade5ac82174d020f0a508ee174f7e23c323acdc5a3205e3a5c" },
                { "xh", "44088676f111a7643ca8f629706d2b6865376979928b63aae525b51b8205fb259aff1ed2110d67c4b9d2047f00fcad87aa5575350e6f292beb13a5ca05e52681" },
                { "zh-CN", "9cbc1a8b5c0ffbeddd04007635b21b4556392e53c1701e88093f2308859a6fabd2b264b248f5d012474656b934496e7bdfb80410d01a1c4c64ac97d88d45fdaf" },
                { "zh-TW", "4d033dd8c154484e97508a665ccbbfa089429b3126d45c2788942a1b71b06aa57e3f02dec5a95be6c0a60290497dbf86d73b2c3f653716eef9d34384ad2799dd" }
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
                // 32-bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
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
            return ["firefox-aurora", "firefox-aurora-" + languageCode.ToLower()];
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
                return versions[^1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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
                if (cs64 != null && cs32 != null
                    && cs32.TryGetValue(languageCode, out string hash32)
                    && cs64.TryGetValue(languageCode, out string hash64))
                {
                    return [hash32, hash64];
                }
            }
            var sums = new List<string>(2);
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
            return [.. sums];
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
                    // look for lines with language code and version for 32-bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = [];
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64-bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = [];
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
        /// Determines whether the method searchForNewer() is implemented.
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
            return [];
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32-bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64-bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
