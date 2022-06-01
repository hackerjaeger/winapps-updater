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
using System.Net;
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
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "102.0b2";

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
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains<string>(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/devedition/releases/102.0b2/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "e98e974ab7d3c4cf23653205305ce435e0cf4ab85a90a3a381815d3297913b7549881285699e953f96b57ebb2df3dd9c074b9f10bb40684d876cdba6978d8ba9" },
                { "af", "43feb3ee529902f2d19ca04f78fcac52d79d2960f9c3542fc394e020f9e8d846b9170de05ba1edc351984025314b0ba3aaa48f58ad760f6e171721c8a6253206" },
                { "an", "a459d8d00fd440dc985c742e9c37f5debf7c4725d39cbcb9cb5db1c8420ef276313851907d5c85fef57c609474c02f412dd548ca1f49e993071c53290c9b6d0b" },
                { "ar", "1a4beefad9565a589c14f7b60596194a68bb3bd8dff18d46ecbf587609b04ea9891b41f82178e2630650dd7e0488e211e421aad7c0f6daa17bcea5d603fab182" },
                { "ast", "d8d7f4d0f8fd232756711c086cdae6b99f833a7ff8c785714272e564c4d85c3d76f4f01234263f7062ad3d45ec551923f76ffcdce9d68c3ad583886ba7871f4b" },
                { "az", "38dd50e9e2da19cb69aa4a00a64aaa71e927a2a2f691d50a8408ae64a2c662d3e224525547961e8a4fbffc6d847f977ebd4db148e897b16b6eba27adcc505847" },
                { "be", "0b7d2cfbf4912d5a849d511b874efc04b0745b6212567cf8e8b09a78dce3741e1318d2f84585c6de6c6364d61044935e347e74b8281a9e52f33e3126dd06e04e" },
                { "bg", "622d223afc5f2fd0f57b30fef2856eb6ce36c04970fe22fb8820705187e98d74f2b49c0f5b079c448101996b814b03c8555527d0d30766337d335aeb7961d217" },
                { "bn", "9c43a4bba1bf728ff513410c45088ee243dc49f8d3f8dfaaccf17b04b7e40aa4cae73bf07f914652df934fd22d361381872cd71f8a3bd2e015e74d2337948904" },
                { "br", "7d8e8cd4951614e4389504b0179ba9f01fef3ff0134691b214d377a48448756f193e3a6266f10d2e3179b7e74dd02ada12003bf549a8d52e4a7d0f2039577e2b" },
                { "bs", "a962d323aacba547400b9ccf70bf1423d5e0d18d6d2ae4ba5f5934fea94621ec608e0d2f1546752bd903ea288a93f055aaa4fa10fbfac1887eda740455422e75" },
                { "ca", "2e8cc2621ec73415842c64f484fce3986d2772856f9fd302b0c2e483a20eec4c0f10a6ee49ddd14abebc4ce106c61a5a7411d6c9428491d1eabb9aacfc5f63cf" },
                { "cak", "6f5db86b91516edfb693084bce6daabc2880b0b35800748348e19133bf296a452c391d34eb99a19ba1a39039335fb8449c498b1858e5854059603c204382cc8b" },
                { "cs", "46a9e6ab4cc734b9280cc472529c98668bd086b9d702dcc4a91d660d58aae3cfd80e5a6ad7eafe13799b90cb3a99deed57f1169fb339fda30c78e5700acfe02d" },
                { "cy", "b1722e4608176f919f26db00aff69d5dca8baf579287e85dad035b05ed801a8d987f612f5c8ff19b883f4cb7713b6cae2fce4ec42fb3d261bb5a490432cb0c30" },
                { "da", "c322fbc85a7e5d49f1ccc8d748b82c6750b5a50007e783920513dce6d53d1f8dea982b578078248a235a25ac3ebf3bd53e879943c12fd2c81f97ed9f884dbd80" },
                { "de", "cd8af2b31352b189d684ae29cb8699c4cfac5eca90e98903c39a9116e8622718e60c5773aabeec0000eef4cda598123111ff5c33a43028d13912e494988c0e98" },
                { "dsb", "4d44877eccbbf4f66b174bbcdc75b3b26fddec449bd427b55cc2a2d2c90925da8dbd913c008ceb47d4bc0756b8d2735200d756c9b531001b4dc778efbe7bedc5" },
                { "el", "a9934275ffe4cc6ac801cdf504b09930d5e1c327db1a04e5aa03e267db6f81df81b0141e3d38e8266db0ff49203b39da3bbacd7353dd8dc893cb1558bc32ebe0" },
                { "en-CA", "53eca2b07847caecf369d1c873322fa8325e7865fd4d2be8944427e3ab42506cba5aeeb09dbfe4cf54e9c78d2fb65399e8c44121e4461606f0f034c9329f6430" },
                { "en-GB", "de245d2775d5fd2131ecf313ecd67f9aebc9da7122cbdf41c8e061309e0b5de81dfd863312fc475a3b2a7047ff2ecf17d2a7f953efa64d8ea2f2e7a01e89a9fb" },
                { "en-US", "787f21e1c5a77461569c9476b539af2cc0cbc62b0030ec4249c07e0a5c3a2e671f1ba2ad3dc311df80644113075da95f56a7b539ed336cf4d1503d10286c9680" },
                { "eo", "387892a95961e72b41f04e45bec57ce27a36e5e1d9ccf3be5d7323f510c3d1145b389c1277de15fcf3345bd47a33b1987743bff6cfa559f2d1ca660589ba20dd" },
                { "es-AR", "df5b971df3a8592c45e915fae52971100e5995a334085172c8a2d849282f4f9eae5f9c2afd9b9a69f22b84ed0ed5b5db3034d8a5b4bddb90fa28db5a3eadccb0" },
                { "es-CL", "e673276a015c7b63a922ffd5a3dd4685ccb5ae7a99808c104c006624421d1e91f02720ea55e39c0ade088caaca4aeafbe85f3af95b999b06b85d7c014b98b076" },
                { "es-ES", "464b727fcaf969d994c582404481d7b46ade80ab2f6391fdb3e3bf1c3fc5bfd5adff2c63655a6302af14d117ab6ddecb4566ffeb6825dd9302c2029564a8b029" },
                { "es-MX", "16f0218a58c5fd41c5296411f3d269e833c1f01a85185c35a0d8fc607d2c1f05e9b32d42f57b668ad314153a0771ea2b9466712ec29388b83e7194ddedffeddb" },
                { "et", "8b469c06f6ab0af4355cea17506f352396bffbd1ca365252befbb4778940f6816595e591dd96da4db2f9678c2a33d02541a6acdb9afc0de163d2ab8d4c1f607f" },
                { "eu", "ff383bc7891cece328b5d0a7e1c91d03a18812710097d282a046ac8ed64bbdcb77b083a8373d46de81ace077760de5e472bee65258c4c50a6ef6c42fddb76e1b" },
                { "fa", "802e504a3b75cbd7f1fb52d1c8d0e7c38f5c27e1019b54f7dd740b134b3ca608732a429bb73195603c74ce2049a567413aa1c19847b1aad051cdfa0c1c026659" },
                { "ff", "74d20f266aa69217b3f85d447add3051cc663e60bb9bd83bd4a0672631a46f43486d2ba83b3ea213938828a1b9f50ec67e1b3c460e922847b62e6ad212993330" },
                { "fi", "f0fa315f9def7112d42753baf0432df8031abe22ae70f1859f5f9310a1a0aecded45d324b7ac944a13b8a61ddd2cd07d85e4102782f1782b623ca7a649b014d5" },
                { "fr", "c5e0160869a7d396e1293b387966719696b8ad9148de856d0110147f0e0f4df1b58e53323ea6e2351ba9974618a8bbd659f12e2db4ef1c0e606c9a01ba8f8c7f" },
                { "fy-NL", "9c85237c295c54dc41fe20473b66c8062c7d11b19a2b6d2884a57ad5ead9614907e7e8d846c8c35f6e3922fa460f415dc3b70d74821d2f8a9b262e496d7600dc" },
                { "ga-IE", "9a51d849dd21224deef93661962fd17206278d95afb4570cf9f1cd6c117ee524fe51be4df39cabc404a6d25d6d6b363019e69b3995742ada03cde40ad587f386" },
                { "gd", "8765a4363be840e749175b7b0bbfbd07c68b43a2d406d077af16946e4f4e14c7e689ee37b0f0ac267691dea502ff843a884e736cb1e1f8dfa89a6c0a47c8d30e" },
                { "gl", "89debf8a75e42de075ecfcea0ac3b1b07e12bdc40c9c2f0de982a53c19dc8e7e52ba2a75c89884396a53f8cc0256caf4803839c2027736e9b199b9bf45bda03e" },
                { "gn", "feb67bc59a0fb42f40a8a0e96f1e302754191dfe3c6349f09a05f1b77da73df25bcb0cdbd32e947f83e732d8916f365d48bbd9763e02d60511a01c309f92cf21" },
                { "gu-IN", "aef245ae95f484367c9b8cb130c34cfa5c609a08b6a3a1fa077b38ba98b6f552e07c4ca66c26d43569796bb4340e4c12a8462df7bc3147925e499c32d35d88e9" },
                { "he", "35c259816606da26345c759fdad32f21e2b650dfe9daf3888131fdf13bfdddcc1b26f950c6debfb878e91a589cebf1cc774d98954ecba9e23b8afff2c1ff41fc" },
                { "hi-IN", "f824786e2531d483d63771bb103cc2b2d8cf1eeea4bc34f455360790ba0951b5940aa14c1d7b56da9340461c0ea4329c141d72e466eec164e2a1fca3500c2f41" },
                { "hr", "07a4776803d11ab17472c23dcd5041d97576b1b73f3e705dc8918017f21322c30d18db42fe64e76584bd551819474ee8e751e2c2c000d25052d71321c793e714" },
                { "hsb", "8a828b4a44c369464a53e34b81971d767c2fd9ea0e570708708e121c42ce7f07e8449826666796156ca864ad21c4363537529179ad0e7b57b15c15b8030cb22f" },
                { "hu", "901689346974572c27d1d8d243c26d8a6a10402d7faa8d8a1d59bbb4d9bfb3c0dade82494e1d9c71ee80870a129abd4d8e23a8e21f4d53870c9766d5ca8afbaf" },
                { "hy-AM", "e1ac400cf1a32d9b556d4cf91eae1952d9a5ecb6f52d25f42f46689498c9763417c08ffc06d089250aed369cbbd3dd87364a1ac63e62f199b3ee27972d8e4b19" },
                { "ia", "3e9e05894002259bb21bfdcf3b76f29b79aba3fe31de9dd5ec31dcb12bd854202663f8fd7a34be552e921955744bbacb86322692c380f1de7d4807909b1e2ed3" },
                { "id", "b68ee82f53b271d91d8a431365ebf2d439bb293669863e4aa939236facd332e994c23ba627c66423c73562e448ac4a15f8a9b60f4dca63bcf0bf72c50f365555" },
                { "is", "78e678aaa3dd109404e5d04703b5faacab554489cb5bf13a0121997658bf9078415f31c0437ed14253a8897f79671acf946e13777d72e82a487c68b6d6c678f8" },
                { "it", "22badad3094e6bc6f8e4e665553edb78f963bbeabad425fe2156acd00433aef90ab85b001f2214d7f65996b8c46409ed7cfa2dd31aa2cef72518a7354cc6966e" },
                { "ja", "0fdffca178e2ee0a664e0ccae0873894054d0f0ca213353e5a88b34dbbe2e6fab2000f453e2e65a1403aed35d9508ea8a07dfd3edd7a6c5210796f65f92ba735" },
                { "ka", "cbb04ce1211b32944ac5ef66962e6b04c6acaefb6ee55f3b09763d69120c0ef28c8e9bc29434958c202f7f5d356537a6882da22f4273e6fb3d51259f354a2866" },
                { "kab", "9222e36a218a0702b7c7255a1c59ffa70264c0d714343a699691be3c57d4b7729ed55fef4e4df32056581a960ad68cd11d953c014236b246930b32bc7521b798" },
                { "kk", "6efcf39ae90d95e6eedbb6f9a0c72c1182327538ce772c8e0210d6e4f26aa48e5bc5dded3924ad1b70a281ff2f8ff0da1f0be5274634248383271306abcd5cdf" },
                { "km", "6df37f2a6847cf2aee8f999468115394df45e92660c40ec7ad7680e02363069c950815e921e728ffa0331eaaa22d7cb2cb3b3e03c927af893ffddf068b1448b0" },
                { "kn", "4e2893321cc3b3493c12c7ec7f978978737d9f6097f0c257fc45e63d8abcd8652852617ba1b093919c14371c21a316dcb4feb87b044f36a2603a383adced9c1f" },
                { "ko", "46a07b37d1f76ff41d2be18d03946d98e6482c2f9bb69a42993268aa11ddf94c8ccad6ffa559009768e34679e9991b32737d9076af4349ec88cc920a170d5e8f" },
                { "lij", "b607566832030b730e232afca860b5e28b98a10d48c9553042edfefc41eaffcf5e33e969903b2b70b090ca43e2f35fd9c41800e3810900345095fb87d2fc6508" },
                { "lt", "bd1446cfd13e76a823251447518ddcc80753a8896d2c374dec6696d176031aa03283dcb6e4eb16c21874496dcaad2a4957e57edffc16b9e4dfc56b955e596c97" },
                { "lv", "59b36a20f109a3621777f466ccf53f6fcf04d283a398ac48cbf777260a1262d78da1dc061bed3449b4965f478b328493bed344771696ea2a56f450faea93871f" },
                { "mk", "3430542f2e837a42831619721adf2455cb8c2357489e0bbff130f2291381308f5b12faddfde75d8a1b0dbf84b464ee416cd69af680ec1c521b8e453f3bc31703" },
                { "mr", "2b813eff1bbb79d18d869412b349b6839d9ad27b494b7e581c8fe8ad60796f05f7b70900c5dbad420e28390aafb67d25dba6938712b9eca31d27eff13f961616" },
                { "ms", "fe8033c9f3d770019aa51e244bb3a4296e4be627ae4ebe515b1a2edfc83c98849e18030f777eb85ca7ae97d926235f26138790344224de0a128698ee56944e03" },
                { "my", "8c96c0eb499e8c61b04c37fbe659a09c801d9f85a365e4a3ce2e7e70f08ba384d6f030ae5a754b57daba1edf9bacff096d888ddd354b850a0b9c083085127ea0" },
                { "nb-NO", "56f3fba463c4a6e49a4b51db45ca9cbfdefcff4058576686cb84941fdf8a875d9e3ee2ad49071631e82824492b063cc26007821348fd58c218820407e3cb9f45" },
                { "ne-NP", "debd757fe73805d6c24f8a16f68c3afe02cfe816f70aedd6ef7f2609d0fbf657eeea16ab175443b5ae0c56df15ba6f426810cd57b6f375943fe9a96d70a29334" },
                { "nl", "882b726207c25b2ef9990c64891e0cfcf99a22783a6abe79043d9a5ea867903a593da160466b60da278f3acb620038dfeb64e648002e1c7a95246e8dd6b5fe55" },
                { "nn-NO", "873f35cc44afe67e1fcfa6965a74ab7422388e3f287dce9d01ff4e964d3b43ab456a21d32178c25e919c17d9ad58c58805d1e2f5a15999d94525e7e3142d9b66" },
                { "oc", "e513c73940e3eb2a70ce0f17bf24c04096f3d9094ba863fea443d26b8aecb4851eb4d18a24f885611c918ccb187589794fcaac29d5a9fc4373dba0bac4f8d97c" },
                { "pa-IN", "68d63284c51a1e39689cf1316dc3e032addfeb3d74965e661ff7639ca373eb55dd6d029fe641860c7a38396e3f7bb3b9159ca22bf11f4f88f3291d4231e8c787" },
                { "pl", "d0cb2263b176668337c3dc062a51c8c00a890a01c39d76b7b7bd260a6f43370f8d2eeaa0f84689a7f078c1af5e485ce6d41b0e0d53e7a3e58dd66d478f1b01fc" },
                { "pt-BR", "88e01f8001798b19e78fa830885c2496886a2d4de320e783bca9a55e81c7e7ba62132cce1b78a1bdf64d30f42f94f3a9a2cb57bf83d0208695267b320478c52d" },
                { "pt-PT", "16233c33a982514b1a34eef677629cbab99235e80711a18f531bb6adfafd07c6d8d38a87e1f8fb27e70db82b6b242b3f13f8d9d8476d65d87beb69119318111e" },
                { "rm", "ecb19f69e3c02780e68c6eb7aed6e806a7a205b6194f8cde301bae689c02fadfbcf0a39445becfb5982d6ee06a3553dbeb91f9cb427858c2d3b43314871f3910" },
                { "ro", "5e9292816ded0538acb63cd435924141ce8e2dbb2c03dbdab1ad31df49c52d680df63e28e961ae97cc6c40acd675c5ca2ff2999fb3516791b5e29591f7ce6402" },
                { "ru", "16c0f5fe4889dc0327a9a6bf04c71c3cf0546d1013f038b4b395f1682501b3db3e92243c253e4fc59e1807e78dcd1aa8813f3ce8cda6757651be0345517d5fe7" },
                { "sco", "0173f9d4edab4dd9e88459cf038d7fd260cc73e75fd159b4da98bb80f012ec9f167b9357981937335750210bed8a27aedd3490bf917e73cab5ba8bcb068589ac" },
                { "si", "c222ae71f665efb1c30e8f1a22df80870990cb2e7a2bdaa9ed18296c94625206a3aaecefa80369a65098c7995b83649188dbc5de72a180e83ae7b50bbe77c8bc" },
                { "sk", "ba3c95e01c503ae6ad9b6be35d86cf820e77b388466e83d85bb26a17d1ba1bf8c772d7bae8bf5990245d1bfc021bc83b4b6ba777df96c03d25656026c39c27fc" },
                { "sl", "4a6a243ac8e2d28d34528569bf77e2ff543a368ebd7e9e604207ff41ee2dff17b32be27537aeb1a678f88dcd487d7c2a5e1c45b1630220b9ce6ba0668b8a3829" },
                { "son", "ed11ebd14d40a37fbc9f11e4dc0632f75e5eec68187c3a0723b143bff077b8fa16b68fd7e7000f66faed7a081f6068204e686531cba0b05d7d5ee4b7b2213bfe" },
                { "sq", "cc0ea262dba0c5d3e6e5180087cc89e60de88b184e408d6e6981ff6e07706974f4eedb9af2297ab817ff2d4319e6fe9605554b477acebf2d0f803b084d68b741" },
                { "sr", "5056792ec34b102d61c1ad33fff04ded5bc1fa897b657d904d1dd1d0dbba44d284bc063d1a34b053d7d12ded3b21153214ce439eb3dd49e09fb7af49131e69b3" },
                { "sv-SE", "85dd4901bceabe5bbb842945c68c610924d001f0707d763d5f6775bbe924725cd2495b9282d6a5e9232c3fe46880d64129382e6a8d33cfaac06d96bb05c676ae" },
                { "szl", "54fa8c03dcbc097c0174e575e208a762845fa7db5f0f3aa95996ae404a5c0b6b2bd9151f5a4a55f3d9aed63e725a6e285a09ed5c16effb7061f75b37f6e384e5" },
                { "ta", "9163bc16504dbaec2534f67cc5e167ceeae7e4419facf9992cbebfe85e18707a2fa40806e1b3328dd5edc0db820587ca0a95de1223e90aff2d4b596d9018a277" },
                { "te", "71ff203ed0b0e26d8aa2cd51951d76c561e3fd6b75daae3e521349a9b1c2a3267ae3ce8a4b1357c24fd6c1d16687d9754d7759c4f50050072a63a36b956ea8d0" },
                { "th", "e605e1167208ff13e80667045f1b7227f4b49d1fbddf804d1bcd2e6e882601a3d5fdca2c8b5a837b709c7f27f35822d33228b9f6682a683dce604c0662bff1cb" },
                { "tl", "011703855d8d030e94d9e32b7f0a82084b5e93d0b2ae588f1a21e6da970ad8ecb9d680a6d48d0031c980c4eec2e4aa9642cd6916bb03b4bdb88fef6535c52fce" },
                { "tr", "52527c3071b9de9f9be8595d1a7aafca36d8cb75089625a42f780fc93fae980459ad5b6bf1f383308d8fc3b114702548d861083689dbbecaba015a6857afba42" },
                { "trs", "fcd7706997e50af7704949b51fe7e2625768914a78660f9ab8f51da97a708979ac0dc99369ec8718cd767d6909f7c6cd72168c41a43d077d575b95438a84125c" },
                { "uk", "11c93cbd98b78d09f0aa7ffa9d7eae37ec90b90d790a3f6211dc01ed76ee4cf19b106cc394b5940e5bf35fc55dc7f79ea6635d6fa4124ebf077b17d6c1de8daf" },
                { "ur", "3e42c75443a6b4b77f701be531928443984382bd4e98138fc5a76eae765af7615252632adc6552e5a67247fd3af6d80929027e69c81e0e7ba79dc71d305c10fe" },
                { "uz", "c4bb459899549aa5e5c7d45007863a04202358bef7f3a5aeb2d48dab835a7283d101a8b3fa8f09e732491fc08c48d8b926402696157fb4acf38c9dab58aab42e" },
                { "vi", "34e862266a4eedc05261f3d4c29cd2282169250a795f0238b995f0aa4a26099c8c77df0a4166080ef4610cd1856aea01b64b04b7871ce83e59dbd06d72500be1" },
                { "xh", "843dff4ab05214ed77dad4b584551386b5a8f37fea541e6af0e2d1a38eeca316c0cc4bcf2bfbbaa7424a5e5656ac814102e9554d48d222a4c7190bf1056b3ce4" },
                { "zh-CN", "407a767dc67c288a0149b4cdc01852424027c44dc20c9ec08d7e96d1bec0f6c8fed8df4473885439c8f1e063a558bdb91b3b03f26c64974ca597257677094c2a" },
                { "zh-TW", "f235053b962038dc930b03a8ea0e0f9a141e88d3c0676a81f418d06ccead56571c85c2d3443ea7226876679b8d4b256271d8483117be9c333b3e28462cf15399" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/102.0b2/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "b0c3d98e104b1e762de48fb5616988c5d4f5ebe8cdff3a90a9e02e1e59a55a7195b7c85b212097d1c2030349a724265e05633fbfe5ec3a444b967c79b391f0d2" },
                { "af", "398bc8d9d264cf8addaa82a56875df94ef521feb8366b4a8c1f0b58bd1f8da08cc5d7cda1ae8681853c3a07c7484caf502a4dd4ae70aece4ff0e5e54e38b7c9d" },
                { "an", "6df63a494fd1b4b2f3c1a9bf3054fe236293f80a69ff12eaed28b5b73ed65504d668f40cf9339520bfeb89832fcece020b27562e7d2cf379cd661492ac4c654c" },
                { "ar", "60b5601599dc3728176825f9a94d7e1dc20a6fd0f28f65a4002c9ef81d3620b8d4974759e5cb87e0eca14ea3aaf453564951e54a436eca9544f86ef1dc57ef3e" },
                { "ast", "0e1390d2fe55d97ca8005f357d1684bee36e79d8107a009027c13497035490fc6a4ccf84c04758b3c39e3e809b3b5b31b2c9a2e0c259b20500627bc8d515a338" },
                { "az", "fd97a0fe02e182265abc621a8d384bbe95c2cc3103686d0027564c0cd3539083e6b4969ec8264ef3d244abf3523c4946875b709597e667ea0d8eb6403fe9f9b3" },
                { "be", "a9e06ce41c465dc2d7c19a8eccdc2053346d02d7f9548829df44a63306e19df157c38962a326a2fe2d4fdc1693f23a056cf91a6bb7f8b0603eadf229b6c52889" },
                { "bg", "1cbbcad74336cb9291fda29553e717ca1d349d9d466401f79ffe45bcaf13593e8261d8faf23913da8b4a505db4eaa6dd326d780c8951868650ee5c52159797ea" },
                { "bn", "fef4790bcfce18138c130bfe070c540f553a7e961bdf036e83ea007692ed4f4f39a5b34f5c8a69c105c26fbe40703cd7dd25f6915c97ffd02f9b27cc389460ad" },
                { "br", "8ab44a00fde967359128c91d699acda02eec861d42af6cd139b7ef84ab07170cc117be3921ee7c37de7bc04b7091408cd7272dfea4b2af0ee35e0854f3f371a3" },
                { "bs", "56b55347b0ede2857c9512e6d96a48fcfd95e7defd997cb0642116b7954ac7a35c01553ea0a8983bb62172529d810df572198a370cfeab579f162399149f7f16" },
                { "ca", "27f108651b9eab4f6e98fd709798304925d84da22113f8196daa2c012aa567ca4ee5aabca561a4bed2b25df4508f0b60c2cd972fd79a0ffd9f7f772ea233183d" },
                { "cak", "a82332ab8b1cffd4ccc47854ff819f3ae3fffc0596222c6b4959c372712ab840fea3f83bbaff26fc8befdb07b46a33f68bc18195940d8cb9cb81f231f30451ed" },
                { "cs", "6b748854960a64c000ca4a50bcdebe2ef17f869b0195ec87442453167d857abad4bba082c2f959bbda55866051a98f56ca4e2138e892f51cf0f66391918fb9b8" },
                { "cy", "e133276dbdf62f7362cf2ba6afb7956a689832920c990b7971b8065e6c9fc4b9c402007930e77eb51fa1f5c0e88639dd51bc009c617853ed38bd6c5df2a82429" },
                { "da", "3287506b63ef89157ec185cf350a7ef7c5f122d84e1dbfd5ee6256f958b131dbf0a3513e39af5604fc4e5394dee070484e9bb9b1600d0bc2afd88fe68e05337a" },
                { "de", "cdeb06827346ea7eaaf9137707bb9dc87ea3e67f51252cc775671a51dfbf0f3c194fae7eeba739c53a9a66bd1750b10359b6ada43e36bbc27ece10596c650aa2" },
                { "dsb", "f34eb567bedfc35fe3abfed56567f3e263cc03300599694a32ee89f209061d4e17cfcf08afc601b98202a42286768bd857965f957b90ab4df2524448e66ac12f" },
                { "el", "8b725b8fb714d4a5547141d1965f2f4a8055c185b4df747e3ce8f9abd896ee783d79490a2d80dc569256ef4c6d909c04b1a8d823735fb8d106c9a5aeb164a01d" },
                { "en-CA", "1e583626a0d7f167bf71aa605bcf248afac99d6b2facaf97a6f7d64733322aa80ceb2a8cd2875f9c9f09f388f3062d517bdd3c15f4ffec450aab3b2e59c58b6e" },
                { "en-GB", "7029df82d3e4c22c1c319ab1cd368620e3834ea3df9839bd264100a7dd22247ecb929dcbdac2733e2ea4d78d5e6a4e262d09fa761dfe8f6174dcc2dcc6a65ee8" },
                { "en-US", "3faec2a9e447fb981c69f11b4ac0c886a91f17cc8154526d53523517fd149d28454ebee2c0d05449bb293b4687867de1a86c130343b49c804b0afb4f27f57d61" },
                { "eo", "2030ddf321d6b529d4d9dc4bc1857c202eabdb1ecd0a0dd6ea4df85ae4c1909ae59feeeb7009b2befc741b35d139713f6bbe13d9c99b7da5f9fb4115baf70726" },
                { "es-AR", "d2d3a9cecaf33c4741e65c198593ba7e014d198a4fabea5a7b739661f9a6ffb95a4ae3415a0c0037140857e4f21ac7614698aea669df060162d9f2ac9b9afe16" },
                { "es-CL", "71e667c8a9a590ed69fc7ecb94eee5d5306e19fb3ae828cb6c2a28fe11b97eef94452f8685b85e65dd745232f16524fc5e6eb3bf32dea594d8bbe7b3bb30a4be" },
                { "es-ES", "e1b7e1dd397c1d65570e13ee4cdb4518882a3ffa035a3e2ece324e976eb50648a865d45b3ceb3f36691a2d898e1a9c7bfab360385ef809d9ceebdc01f301edd8" },
                { "es-MX", "6340d8720345f31a014baba42ef4077b7ae6ed5569117f2e92f4c32309e2fdc1b22cb0fe59a6c137b674ae3d06a086b877920bc00885675b4a3159cf3b1f6107" },
                { "et", "06257458974d19f710ac7e8d79df821e9916755c1f1a9a1fbb64ea0566b4c003e05f179a0f44dcba4ac982375cbd337cc3486d46430e8f490baaed3c246e6aa6" },
                { "eu", "6bc4e26f25ff02aa45138e45083031082e51760f09642e5ef379bb45cc57f73b5d64c04e6618d5e827952b8e460944cf6bdbcf3744ecd9345010b5259325f706" },
                { "fa", "94b587195a1ca78eabecdaf8def71c8f4c7f9fbc8f92986ccb420a6930ed45c89d6cafc717c541f77ddb7bea3d1a67b9e0087f614443558413d16279f7daa808" },
                { "ff", "86a3d4ebd71d06a17391d4d26f2dde1a05701ebf5a8c8e660665170f3de94cbec71451a69a98a1e4d6726d48b8931d01469803e9282a282d5b66a5abb9556a0f" },
                { "fi", "4bfd649beae0f4e657d9eb486161b03c5cf3193f98acf1c4e407464a5898803dc8ccb77266a26fbf1c0551a2ba195ed11ad3a56a5f455a69bc9dab1edb418e95" },
                { "fr", "8ddf94099f09b19f500a578bf27a744fe78aece577e70e476dc847278783776af5aec00a30ede947a5cd3cf3386e230a248abf1d99c52d12c5190c3a27b7cf14" },
                { "fy-NL", "04e7d7f9bfafabe06b45d9b594d7b87f7c8b9402bdd965e93ee2401e0a06b7b0781718fca4b8293a480a6386dbe4b0e9dabfa6304be64e25f186ded6ee12d9c7" },
                { "ga-IE", "58fa8c44438f4be0826ccf99060ac18dc2f8c98ff4a4c39d5d59da51e91f01517c15c41d32808b033ded9038f3ceb5d5693073b266fd936fb5997fa0d2a6181e" },
                { "gd", "eac2b7d43b06f403a014a23141d1f5503a811bb89d01cdd7f103c02ebd86c290e2820bbe3bb891934c0a3a1aab66feea7cc15fc67470832fd1c77a56e9119bc5" },
                { "gl", "c5e87216e24aa22657ba3fbdbc2b6f66fb3ff865d4d2b1e38f6e4db07ad8a7412880cb088ab9e0bcdccddbb5e4ddf1dc12b2925dc1831c9bda1b040ad35f49e6" },
                { "gn", "27f5a81a4013904593180ce365d2fe1eab80e9d2c234cd069d77f4968d94524e82bb3ed6244fb84b02481ceef5719cf816e2dcb64aa1100e9df53311d3e2fc0c" },
                { "gu-IN", "c8cd2fb8da7b14f719f7209a2056f5e79af82f1f827dbf7c51948786f17631dfcb0deb367ef0a77dbf70151c70e610f793fcc4fbad09bd5951b7b9d8eca334eb" },
                { "he", "5156ef666fb28fc0a2bebdef752e19da20ba7df994e550f481dc3125f23a0b623f7a528c635c6eea27d813cb64f64398f2f0600b642aa1569103d8690be22e1d" },
                { "hi-IN", "ec4b711496a5747c770e1b16c2dd8ea1f16659df1612bc706c19e299072a6af2597a73d56baa9acf5d54c6719360c7a9a5a546584c7dbe9f305edc54708c9c4b" },
                { "hr", "41588beb7a2dab20b0f836f5bed332ad3399f244c554bba401713b91b7820b79244281dfce5c3d019696733202c07985a8ceab8058fc1115f6d2a08ef92dcdb4" },
                { "hsb", "fd20c8c8bca746a71eff4b58168f12f3ebf93da70ab851a80f0077e8e210d7d7c477517c8d197bfa739bcb861d1b99d6a9a3d2b7bd719f76a414af777569d736" },
                { "hu", "7521ac41a9caefa07d55e26f2553389d0b5af68697148cfbc4efc4c9c86d9c918149e53e018f676cf4787b74e960dac4a71a407034dfd4688e2f6d15374d503a" },
                { "hy-AM", "ba0d26844bce291023f06ec44112227dad290c6243be957ecfa2ff06b4a6d48c988780308b446a271415695a1bb11214defbd32199fe546b2fabd7f2b2c964cb" },
                { "ia", "3ad8ca0ea6573316f5bc487124ec82afd049fc4bbcf9bd319cc1e9aa07d30421d12a0ae79dbeaf41653494dd17e54fbc496f6d402f4296be442f79dc258ce10c" },
                { "id", "f254f3d580b7871543625101cf77b43f15c169c6ba2e77b5ac0ddf2ca061cd2bd1afa338845549a536b348bbfae1126e9b4abd86b5596658371359b8409d1027" },
                { "is", "d4d1fa2ebc9828cf27e9705b8e6765669012c6660dac62c3728062641bf93b14793338c3d753413d9100c9e93fd462c5727b61177f4d27f84c9e1a12712ae32e" },
                { "it", "f38321dee242aaee1f76e6234a0d73803ee43f541ea094386612515290efe69f6d1e73a2afc2865959643ec2f26b3369b6563cfd64be213388647c71cff5fa44" },
                { "ja", "bdc6d3d8273398e08e66ab68047408af55fa053deaca9345a333fbdf47ca18787979b5aa414b57fd915c77d46a011a0b5c6671d0b5798a26a9ae3f3ab73f5630" },
                { "ka", "537b1358dc3e6caee3a1a0ebb1b403c77fa93f93e8bf14f9a35e72995ebc7ab4817c7a4381e38ca7173a4aefc3fd8eaafe218ac1d64de5a94e23772c058fc03e" },
                { "kab", "199196a1d1ee9b14dca12b2d78a84f8c198c7c1984349483a9e697f1d412dd7a3ef24e92ae348b23a2619a22ffdf9f39a7438d18af0d7dd331ff39b291ea6b73" },
                { "kk", "9dfae6289ada2b192bd82e45e341ec7fdfa1c78c2618dacdb55c8154fb7ce23288d8b38d33f7ac8d81cd72eebfd1b9bb2e4ec0035d482c4097a7815c9608ba61" },
                { "km", "a2aca6ef623cd474596d9975971313cfd7eb190174b0aeba2ecacfb68ef6af261e4b22ac28e55739cdc680cd94e769a203562fb9e00ae0cf813f8b4e2a9ee481" },
                { "kn", "2bdec311af6a88df0c9a27164e066d92b875ed4cd66283569355d481c16b379e192343e990a43ea98f99501ab83f821756577608305c84fcc73a39f2a1fab2e1" },
                { "ko", "1835465fb7748c0ad2e23b5f182a3d92236108354909410fecbeba81ed8ccf16ab2c20572f7a78befdec3f81ad0cd6999d9e00f6c7681399198aace336731e8d" },
                { "lij", "986e1fbfbcd6e96fb788b85015f053ee57a835b305a923591f1133da8bab3289824de9cf1a8e380cf4fd0e896d6fa2526ba5a8e1997f2b068097d8779b34a662" },
                { "lt", "34da6d776de07691a5ea6acb552c9e4ba7dd1479495e65ca220b42596fb4e36d7ee9f6d64ec349fb897b04e446023bd75ac9ec67a6c683c9e3f536de16d7bc14" },
                { "lv", "4985b68c588040f92125674d8c00bd93e77eabd6ba2cee692103715060be4f94e6a2a614feb2656a3f9e1e7879e135b300fe4ff7dfc6565007e16336b487e8a1" },
                { "mk", "fc2e00a25e2b076e56fd4b667fe00afaf39bf1f022db76b18ea8766a513523b91f3b3857e6fbcf8233ada63f0bba7a9455523b794f1174ad85a87b4d6817406e" },
                { "mr", "cfff277f2241a4e46eb6d17fcfce85b47ebbe3d7051a0b7294bea62eca27784d4414685c14fd2ed26878ebc94c00bd7f50e810b20f7b25ab95d655e4af1ff856" },
                { "ms", "e83690b5a8fba5a91ad4dc1dc4c4caeb4965a03f36199d734471eb8d51bae4d090a37874865a74ce13955aa5eaec28cce4dd754d2ec938b78bafa1d995566f07" },
                { "my", "68fd9667a6426b946b36e704b1843090b6cb71544a668fc2b66d8d582fb67bd1e37474d898d553f8bc39cef03d4003f0f1f182fd0fbc1e17c5c7e60fd4638270" },
                { "nb-NO", "66d401a647c8f86f870feac77bc30b7a8a8e47f54482b3671d0c3f12940a083d5a3728fa2d92f6ccff0598de50eef5921093ea6ed596b36842b6050491625c21" },
                { "ne-NP", "1eeda79b9b03ef5e6fb8f35ee1a2c2c97476eb926988c32dd8879e0758334b93426acced90080b4b22c9a1aa3f14ca1ce56dd155d2d50e0e4077019eb4644a4d" },
                { "nl", "fe4fb1507dde4d5ef445228d0610d49c7af7fc5046b5b2a4fdb9e9aac32ae0d2d87f1f2d6fc1ca2c11b35ece3f317d797f8c77c2186f9c93a90ada99f728955e" },
                { "nn-NO", "b17f82679b1eef0b78384f8c3eff1ed6dcab3b39bc3ab8b03ebba9615345188cbb84892b0d90b466836fb9087bf81d0dd3aaca0512788bbc185f2d7046e0c34a" },
                { "oc", "2abb8d5f8a598325b4f2cb8b434d7fcd991185b7a0bd5dac8f912b9c89dc60b675396fdb9d8c77eaa806329fcc3152218e5b0a39037c92eeec14e8506bec19cc" },
                { "pa-IN", "39997b3c8203e53b36ddd7aa0ba03adf2f0a5a93e38728b734bbf714aa97e947818240dd1bb12bf6b1eabfd920fb06c1b2e9b35673964faf03d24cb518355f5d" },
                { "pl", "cb6b4a803d8a67d1b49a1c9ae5258d13ad77c098b9c57e3e2378e9c8a1677a9ccae211e64759c43983c7c4882282f35ee226d7d124b07ee1a124a5983d3f5814" },
                { "pt-BR", "430591dbd867a4e08bfbe961f1c20bfc246a24ce0c8e2fddd1088aea8310139051833c64e32c978e1697e7d5928b074360d5324030283a9015d414d384d49f5f" },
                { "pt-PT", "426cccf5817a60ec275efddecc7bc0b40e7dca68312767f57ba366ae2788c0d727595b8a809267398408a78fe4ff761c41d992018420e7ee6c34fbca0ca2bd00" },
                { "rm", "630cae4e07273909b82d325e69f8f4a3945ed8b2066c2c60f4175859468af4576b73a020cdea1612b8dc681c7719e6696fd74902cce80eca4386386d9ba8c4b4" },
                { "ro", "a34198cb0924668cb87c7ec88a53e7dc40c924caa0652bda49de28765b88b8610e7a5e0e58b24ed06d5ca02e733af885e945d0a31cbc423bce4f058e1aa71e63" },
                { "ru", "1bd81f5091aa8d3103891664e90e2a20a9dfb8a04ec67e1a2e1ce561873059dbc24c1d36a100e3f3ec07c080676b79b2138bda15db1ffd868b70428194615d94" },
                { "sco", "e4211834024863e74810ae836a9be3a09a0e22e12670aa4f419c8ef63999209e0253ce1569f4582008a0df27c0a881569cba409c9cf010c8c2d5ea00e95d8aea" },
                { "si", "c964e060c34981ac4a760cc2499e6e5c26dc947997c32ca2eacf31074ad5e0f8981a87bea94987b995f72d0f772c73822120dc685350aaaaa426e05664d4775b" },
                { "sk", "93a991af15603140e245285d64521371bfc34b9365afaf057748a88ed02865d9be6f2a766d8ae38bb9d3e939dd5448a8ecdf5bd9845e0dc9dd867412f2eba539" },
                { "sl", "13ddf4e49b48fd01599b147c11ad7f1ed13befe57e99f9302b8409652981d327af70b89162787ac198b7b3c27df51c47a4005c1fa4db4a04a2ed8ae3a34a6eb2" },
                { "son", "07983a86f5eef5200e82f73db9dc7d3626447de04db6e215806a942d6508158fb18958c0cf07abd2a9457076148941618b7cf548de040c93b67adc7236bc8351" },
                { "sq", "abddd1d62aad426165850960d63bfe5411e742b52f058b8be8b6a57b84c8fe03f1c70c90e422e914952ea2b24996e1d3db630944e7148b84d8ec6a5450c43fc8" },
                { "sr", "9e13a841dca18d8004f840e36cd5cdc5177a068a3303b6c51817685043eed0b180d03074e90c9836567f34d0b3e594e5435493a2ee25a371d48a8c3454f87daf" },
                { "sv-SE", "d0dd7bb761f4f61f118dbadafe2df39682b79ec63f042ba60eeca709194307c8bb32196d3eeffc02e3b798028d066df28dcf298a5f4feb7d93608e9258aa8c98" },
                { "szl", "88a327bf8391d3a30be1c55241e45c51b42950cd4bf73dd4b26cb6c8492f9d1b0bc9ab001d2580959b596ece9a37646c1054af752ee31f2f6bbaf13c4de7401e" },
                { "ta", "3cd2f9c4f8f25483ffa69dfacf06589eb699eee9b793bbfd7f90429528d3f61cfbe7e09a936e9e7f9173df3f6ee4263987ee60844d8f123f59aa392ed8680733" },
                { "te", "79b49e92410e54775f3bf9ae75f686d51563ac899fcab9eeaa87cc48791a3161d9426be389102c0973dde2de16ffade94f031556f4f5200a9e7497c64064dc81" },
                { "th", "6555d8eb081aa29cd46062c68063564e69a82db16a7090ef9b8b743520ecfaee0e111a9c1a5c2640512104cc0011f45e9d63d1294667f278e91ce13c7aef2469" },
                { "tl", "a42f3d174cc2ae09d225464d83de4ddb7378947f64eef31811a393c064299b7cda8d6a54874575918368a11b9e63279e164fc6b669d6ee214913fa95ebbb0d55" },
                { "tr", "80fe4993e04d5c9ac1f783e93cb4bb35c47ee63368411ad47155389a53dafa84d61ffcabce4dc9997b97ad69a0d801d4712a9c0352de1f0dd603f051341f1548" },
                { "trs", "4b5302cc24b0d44401c8f34abce95166bf7d103211f1b94852737393243784eb46a38ead30b2091018c1d32c7b092df7e896cee16cea965827ea460701ad05f8" },
                { "uk", "a87b7d03b76ccd7191b763bfc5f20b40d43d6c314369bb35c3bb3f86dc0760b4efba3a5b326a23ed8082c56eeac0aaa5182f2192a0897986dc7a5af8797a7c97" },
                { "ur", "45ce199f48b1f9f754ee7da11c198c564346acd44dce7deb39b22c45c237510efa3351d9b9eb98bf4de53d5efe20faf5029e86a6f8055b0e8abb9c1956b40cff" },
                { "uz", "e475f25bf0753807fd57d94c7afa838b5bbe086ccd5a8596b438f1f44dc867cfbca9c29b798a0329d75a1dd4f93e8381545a667803d5e30ab084b25353cd54f7" },
                { "vi", "b247eea7fc941151ca76b204af9c0e6cf5fc6e2ab815879420e7f7b5b7557bd4c7395803c8f45349e90d851f2b3f4ee8616ad2192b5230a76aac139b1603a451" },
                { "xh", "798ddcaaa5f2a2b3528aaf00c8f36f66ddc27c699c59d3ea51ea8c900403c4ef87a5d4cb083d103bd51d33c4a277412cee6f16c5ef1534ec448f2a386fbb83a0" },
                { "zh-CN", "711b039632c06ce3fc84521e28174ddcae2c26629eb703606f2541cea32a058b6e158aa5e48a5aaf88662d00fb02ad76b6c261fc2b291dda4e1cc4a38bc12768" },
                { "zh-TW", "62bdf1032fbb356a8d1797456143e042e34e27018eb92cf3f57c6fa36ecb096a794ab947c351d2ff856108b775d5b7191353e4215932ec972e1375ebddc6392d" }
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

            string htmlContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    htmlContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            List<QuartetAurora> versions = new List<QuartetAurora>();
            Regex regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
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
            string sha512SumsContent = null;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                using (var client = new WebClient())
                {
                    try
                    {
                        sha512SumsContent = client.DownloadString(url);
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
                    client.Dispose();
                } // using
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
                Regex reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value.Substring(0, 128));
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
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
