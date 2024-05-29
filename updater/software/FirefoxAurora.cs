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
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "127.0b8";

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
            // https://ftp.mozilla.org/pub/devedition/releases/127.0b8/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "3f172fa38891c61ce915b5a55f67afc75852efb4d91cbb9f6b54df99c1de31c8621bde07b2e74d5ca87313840c7de4a86dedccad8cedd8d7b6c3b1b1c3808233" },
                { "af", "71cd77135b817a5d0dffcc981a44badc698a381d2d6ae2795e48ab4644d988ea21b7a9996094e85cdc4c126e08102ff3b4f0dc6f892cebd469111a7024c62d63" },
                { "an", "c5da6fc430d50834ddba263336dc8da26d9fbcb89d9d65070af8853ec016ce006aac392e7afa02e8450959a0c8fb2f2012f954b2cbb9394ba5c75cbeb58e30ba" },
                { "ar", "c1f38cf26af0a4ced3f782914c837b77fe2414cfec50334328bfd28a92a7e9d6bd22b0a28b5b88bd4db7d7e9da4143438d6122cb32e7c1c390998458241c9674" },
                { "ast", "e7424b73ddbba70b7fd11f01f9f03251eb868fc19d3a1a29a5a5854ce21548c589b85d87892ddfeb41e9eae707c9fb416496e6e659e16c00453c16279f756e52" },
                { "az", "126ec53a6aa681b680634b51296e3b48bafabadf9717e8f5e045b522e84d6f08e58b822a421722c94dd414cc0172a8b88012bb892e820244b473674039f2bedb" },
                { "be", "1ed804e804ad6cf4d342e73fc6c3dfcb9d100783bda00dbdffec9a801762cbb1d9831906836eb701c0a975c98e24b8f9646fe9c052a45a30db69fcbd3a983e90" },
                { "bg", "af408f112654cfc160974751da6270c8631924a20549304290273cef3c99ee5fcdd1bcc515add02b2afb2c5cc2f104ec80299a88b3f1dcfbd025aebcb31d2e91" },
                { "bn", "8baa8ef3d4585043aca8499a4890aee064337433289373d3d7e4dd333bf351c7ac87dd042e95b10b781e4b35ef2d16c709c071cdccccfc0de356c2439586aea6" },
                { "br", "0279925863f69aadb1e428a561ee39bdd0747228f76bcf9f1a0e53c5c7dd6216b68dfac5c3eb240bd543d5c4804a184c0558ae5edb110c0f6b3185626fe096db" },
                { "bs", "2c63b00139756a8b365346a0fcd589cf33f77b3ec2f54d7bacc9b6f3bac4f926f542198c7ec9812f4de83b093185f9ccd97a2600bbe7f00a71d08d42374d79eb" },
                { "ca", "5691a77ca5d6b91963e82a370658753c3283e6defc8cceb4fbef587b357bbb1594cacd0834d5672a1839a1edd5c9b53c3293b56e7d2493a759bba39f5ded8e36" },
                { "cak", "8e8ca0894e8a4475eac16c11811903ce3d6104067a9b6c9cdd7fda642969155185c05ee946fee85aa1f6ab1c6f1dc853fee10cd98345f84566a9cb5cc382f39d" },
                { "cs", "edd5dddd7089e19acad4374b0bfe9c2290588b7cace56b917832def8b27c1893ad5d734e583b8c8e91bb118f3d974edbb461ca8f84486f2cee7d21b27e4314b2" },
                { "cy", "ed3dfdbf484d0ea40ba7516e26b4655d72276245f373c04a8cc14bd17023e81e2354c107fc18a071baf30cfa5793615a4db22dcc1487ed759305644484677319" },
                { "da", "7fef35b6f8a8a6da1a37c2570f8c10c7ce69999a741e7c0b43dc003a01174c879d46de736ce4958126e751d782ce5dd0f470fbaf1ec9f7f238001b47ee83e80e" },
                { "de", "0c4f69664e62dba58f7ee457ada2316b7a2e90f72559caa05843496395aa0ca7a5da2342f65241ce13c3e6b533d02a32fb3296fded6ba57c65bbac13244dad44" },
                { "dsb", "cdbf7d51203b6f899f2678de2f64a8e258e9e5c6c78423ac0d4c12a4363d2d67018f51628509b551fb18b9edd42d1ea6a9bcbc5b8942f505be12071070058826" },
                { "el", "82bd752d59444ac33debc7cd2a2d3c12dbe4ac789385ee9653d48986d465f6c064aad793e96de6de64c6923bb02c34731c7c1f99d4d6a74099e6762494c91ad5" },
                { "en-CA", "6cf03f3c950ce972a77372eddb635692bb7e0681db600b89d80e806863f588e6acde9ef8f9048a9d3dc3aee6e684ee3da7855d81a26d4103dbbf1b34b48be3e1" },
                { "en-GB", "9c44e77c3227d5e311b2ef5eecc8181f3b1b78ee0324bc70c0e7839054dc5931ba6cc2def655a1986aa6e5572b90ab1fb4af99b94451d2a864f8ad192e596a50" },
                { "en-US", "e2337bdd5062e5fd1f9ff3a0984bbe6932610d46cd27b581edbfdafd29cdf98b3a145b1ab37045750be5e45378b72b4e6c4ebf02aa961b440074ede395ebbe82" },
                { "eo", "af61fa72c20a32a258d75787e9cb35df7391409aaa1f7d3d9df547fd7fd5fae9169915445c3a49e5399da344ffd3b9bf01125c64861e68231acfbaa6838decce" },
                { "es-AR", "d01a6dd14d734c5e20880ce583ccacb0901344bff3a9786d56509a629713d0974c553618ba4c3a8421a19d2b214b8fb548d48fcb0b09310ae8bd7c470ab7ea3d" },
                { "es-CL", "e161da18f691d9a12f5a43f787f9f2a0365ab0c914fe329ba01d555176c84af7593b78ecc84e3961b997933902c80c751059ff08b182fc11efcc28b1f6e99234" },
                { "es-ES", "b7de6f8e5a0ca5380b8b2358e4f08b48fdf76cc4c9113831e542522e8194841c9bbce2664be05b30d2fb04d9398eb0e7f73d20c0b3b8b4501e39c68e92023cb4" },
                { "es-MX", "e5e2c6e4c510dd60c5f966ae1b78e10241ee07cf7c20e7fbd8ef2fcc24c3eed52c0b5925e26be164476364eeefce84c61111b1a8fde18adaa88e8afd7461ca35" },
                { "et", "05d2c6c968e9eb6e8aa58fdb0d2b8f73170078860b8a1c0a62dffae4c66cb990a1a0862601a828a21700733fb5abf90802220bcf251bd56e80a211ce4776102e" },
                { "eu", "297051340596913632a7cc1f55d0ca76ba162ad0db08235bb84889094318473b52fafa694edf3b1cdc5133a1ed257a9129a61af8bb7caf7744b804ac0aeb2dac" },
                { "fa", "40a49b0d6b9c94b986ceea0967380db0168d1f45fc3bbeaeab5d011629906357b91dbe72d2f67da815d1bb7c5aa21810be4f85ff41b0bab62ed3a49dc61771c7" },
                { "ff", "1cff4511c04d4a185a93f2cab93b628613ab45c1e7029782c81374dfc7e933a17ed75d564c8c4bbc7d16fe68f366d6ac090d3ff29aa8e2ba124105606eaeedfd" },
                { "fi", "46e1675540256e5dab889e47a54fb3f2dac721e9072a16ac0c0693aa8ed36767153603b1dc5bce75626c7bb46efaee0793fb17b2b2f9266a94d47c9885a4bf22" },
                { "fr", "7176b2f499cbf5bb0e6bd43601aeb53982bb8cc95aed442566af2bfcf36ec6cda92882b18527d6a924a3ee23636fc1956d65487423b42b4482ef5c8ecbf2f493" },
                { "fur", "de9914bb6b9c7e5364206749819a79286229feada803b3a2679906d14381cb4c127382ee473c5f86361d5f4e086ba114a5780f2f46dcf1982c4a07f4fbb7ac21" },
                { "fy-NL", "2cdc9d504b24c91b9bae86b6fec5cba73b276601e5aeea25c1ac7198f2b0726de5147f87d30003ea7736467dcf3ce182f647b78e7f9ed1606ebf615e391c53a0" },
                { "ga-IE", "4e2f71864f10715b0d191a8fd5d7e7cd83500f1a51b266524742070047841566e00ef23a14ff4e42795c138f15c61211e3c40cbb4880e114e911b332deff77e2" },
                { "gd", "c0fcfe462cbe2484c2f976e59500e0193ad8e53642a93ed7c83a341dc9cb2238fedb6308b96a7df402054ab5dcb532922529b103dfd5a754bdeab26e03f7628d" },
                { "gl", "638a0e5e1e2b0c31388e3f2743eb33a163c6370e18d8345a8f609cb15991d2a98bff5dd99d29f52ef1183d1d33e2a6acda10b39f64ad28e5641b946a9c6fc92a" },
                { "gn", "8373165e13a8ab19e1f955c980b9a4ef99681683fc1846d898969626ea41820c2634a8597fc2f8334faa86ec0c798b213146fa4dd94b7d573c04a43a5c0efb8b" },
                { "gu-IN", "bbe2343d063dc100efdce033b41c23babca501a9f663f774904e3e1a30f61f9de017e97b9868366a2794c2f18adea4cd34adddf6b67f53756bb2adcf16ba0985" },
                { "he", "4e32e39734294de7720b80db44188870a8b2fc33b1b188ce9c673da857a9e527444a135028d6ebd5809c99951a413a3459c1e9aaff57d24629ebb1c62bae9a44" },
                { "hi-IN", "b878bb5a323091e0101091cffd129acc443583da282114336743ce3b7d5bd4a870cda7079cfc5ce3a84c3c2e086ec78e7ce5ddbd25c4228b18e0c722cab5def2" },
                { "hr", "4025eaaa43d830439c0ce1a8180dd13925dfa485c0d1221b5ff5a78b448e87c3670c6fdcffeca95726aae482eb6a4cd6eee2a1ac5ff246c156daf279e1266df7" },
                { "hsb", "ac4590c9e5166d1ac3037c3f07bd22f0b975dc27ff3ae9c8b2cb4dae6e7d28f93b6a023cbc73aecf166227dd8fae8c99a73a951d529297957f48ed2f144ac9ef" },
                { "hu", "16578f66818177b7879915a9898daf145caa4721f5461ae6e945753f760ad8a5ea756cbd3023fd880b1b4894aab18a49f5b9fb7e0ec4cf5cc440df8b88c52a7a" },
                { "hy-AM", "c181bb4d510fd655685a4b2de320fb910b388d76b1cb068e441e03f84b4695094fa47fa907fd38ebf1a412e1e9886526830202c19a9f68c6aca7a952639d0101" },
                { "ia", "53bd4bbf783da24f9ac51a5dff13487ac23d63c52599b5cec7db496bd69912eaf5afa3b4bb7fb9d29fd76fd56795335298ad1a70cc010bda54fe8aaabd008d81" },
                { "id", "0f97beb9072f9479fa5127b302d530dfbde72ba4c425c0da4ce6260c690f28bbbd3715c69dcda04db197e469e123e33f68dce852d8f095ae3b9ca683d8d67884" },
                { "is", "685aeeeab00f3e0d7d215ca7c0c8e42aa3639428835033aad93663ff4d899b012ff54f1e89c6ad98f86d54eb33c5711366fbdaab24a8df61c2486d2f71fb2598" },
                { "it", "60b7aa757134058063cfad482928a3f031a4940071aa3fb2613a1cd177890b63a939637ae189ba54c0e8c0de9a52e5cabdf5f51d95fbce08e85f5b2de41c203b" },
                { "ja", "ca8a7781f83757d5da0a5a9e83bd3451ef222fd27379e5a59558e1bc8ef6f1be5a015b72ec4d9cff4a41c0447bb66a2dc118d18bb964cbe2e661a89d7cdea2cc" },
                { "ka", "fe582b95bdc4199ed35a6c92fcb6a64b88029f97fa92939e42108b30e893b2a3d8052963425133293fca4cce6758ba759c97ebad856d8246fcb0f81f63093d54" },
                { "kab", "f78c31064ebdd9166efb3097888023c646afcf564e5f10c54a8d1064109d87aaf82eee806fc9edf2c436ccae23c444009e7a7bbdc63b1248074362757c17c826" },
                { "kk", "c7e5eb4a755b64516c385a5cb9d1f2367f01a8f1fd86f389f73d75986d037e375ad7aeb8ea2b5376989a3cb24f31c218aeb96ed3f1a8880bd180e89f8f433c2d" },
                { "km", "195fdb83878a4dc6ecd5ad2adefa795d391ebd7c05660d5b301ef9f8f94f263b2066e64bf0032b76a6bae7b31b105870ee7d941850a7dd8488efb4abbbf95925" },
                { "kn", "776cc375af1bb906c7ef5b70c574e04dc38edd3183b277bf3c7b0b9b318d3690bd8662680843698146eff3322114ac986ca01b2ad49bb53eb555eae413798951" },
                { "ko", "901f51d3ca6d7fe8e4a15602c46c9e9e8304dbdbfc3687389bbb32f5d0ba715e49dcabf5b67936ebf7c451f2ce847a4c484750041abdd3b171541a1ee9636e4b" },
                { "lij", "124eddf9018efc05be87d0073ffd8b9e210dd108cbbb222e6a4098951db97a0e86f950be1e6715bde9dd7d37083c08cb96df58ff75138240af09d877001cff95" },
                { "lt", "8d83d45ad9ec0d6ec515b8b15886e52147b17ab11fd69b6b92c731b57e6c113ed615af9c213deb07de0ed9e970021876079b1b377a282b108930529623f475ad" },
                { "lv", "379ffa495f8a22b6be994b1c0903de5a9f5f1470952cceeaade9748d0c1d73ae7e2f79b0b61a3ee5fc13cc4d1e2558ff5965a0a945e14718a87039c5ceb666cb" },
                { "mk", "d0c6ef272092e253f4a9f211a3f7a37db44d8ad3e49084a41c68276df7ec32146f4e418c9f5e0b98c08608042d065e979621b7119fc12569d22c9153e0189368" },
                { "mr", "131b5c1db80bab3b3999e6e9206562e693e565bf6077579c5d4a4ecb2e2b3c3e54fa3c58a3b0f79da21c9816b1d4e67b8b58b5322cb8e84e70582cc29be8dd6c" },
                { "ms", "9d4dfe6ea09bc06311863dc7592973fa45af2e084cc98ca0a77e51d20a7f08b149e254362d383c6a8d76e7b5d2933e35f9f3ae192149a712b79a87cbb905900e" },
                { "my", "62b76111cf2baeee3866251c6fb678c541b3691cc65cd4eb2676d08af7ebfe29838b19ac1160d9a9fb678ded8ddd92463f831a51fdab635224b930637a24df3a" },
                { "nb-NO", "9f491e153c9a5b5a8004cc7611501971c8825e70c873c56741e2c892dc4e70d1f46f26217d6b0720817b2254dc783ec35abe9cc3eafab3213b8875f20f76154b" },
                { "ne-NP", "8d9d484ca37c9f17dd18b65c898ea9944f063aa6b7e42ddfa476809d269a81c07787af67d82bd55e25e17796d20afd6d78f80b54e21a9ea98a2665bd12859b68" },
                { "nl", "77aabc9e2ec2e69a43029b806ebb2c81c99d6b41ad7c5062a055f375180ec5b698f4f7afb754f7501c8a2e722efaba8d6911d2e5d014a6fb277edd8bc2a3c095" },
                { "nn-NO", "d0dcc36e9d8ea5ba0b3effa046834cfe6ecfbc070962f633070f259a641d6715630ab7de86af2c7a4826a04062f621aa4d2c78217dc5aa5ac2c48ff39486e554" },
                { "oc", "f98897d4da435477181ed09ed2a73773c90b3037fae0fcb29828e2f9fd136cd92ff44f159a4fecd24cf1fa0c0b497a47342b2d6f4e7d3c46ebdc3658998795a1" },
                { "pa-IN", "71ca655d226d4a1749e903da67368a1d9184651eac3cfa7f0ad545443c6e130e51fd186c7fe5a7252135d014d1f4205bdd2a716649a032979e43cc3aabd9361f" },
                { "pl", "4d32360701e7e8671956b87e8e4a3952d393ced7459348f232280414965f9182eb8f76e7cf23c2e69be441fc017d69c1f2efb65ad0040cf25e04e40ca9de44ed" },
                { "pt-BR", "255fbc1fbddd5386dc2407adda3d5dc84ced379bf0e07a1a244e23a8e8cfcd113a04e63e0c2381c45b12098e3af9cc72687ba0144b909fe4f10b539c791a5f47" },
                { "pt-PT", "3761b8173b40c784fe8f58a8977b57e3bedf1512546f2c4444bb53b86b6e6e894acb2948ae4f47bad12a3f9ecacb59538bb2e430608dfd407e75f89b89d80423" },
                { "rm", "10281b72b4d6ce367b3fc3d14b71f793a24324931ab090de87199955f492a6512ab1ddb88c375298687ba0ac3f73d3262a87ff22f254d0253a54c3a1687bbf22" },
                { "ro", "7b0c72d7b73d1bc613e77f6cb26f24fb0216fed0a15f8d466ebf9d474f5bd8280cfa2ef09aeff93a222a54054c6faf9154158b675329c903cf5b1fc0bd124194" },
                { "ru", "55875daafae348406508e1862204c7710fc18953950ab6e985e4515166ea91b5bc7895601edb9079dd04f1c789643c0ee18e7f0395e016be4319b67d60e6c973" },
                { "sat", "4570af84285ea01be5440aef8d94cf13ad2b231ae8eda0fb0af001187e12990345eddec14fc9655b9ea4b0e5224847dec0dad23a521ca4ef02cb07b811f6d7e1" },
                { "sc", "1821d1113addc7cefd8a4eec92980f208ba767c592065da9ed31c3217214155e1081a0af4b728121abaccca73f6309abf70bd20bcb136d1be363a53875c39892" },
                { "sco", "631cba50943eae4aebd860cd3b9e08cce8b81e0257d78e3d4b8a472b0af4e2d234289b46d82b7dfdf2f01e3af0281248df41852162979e74709232a00e17312a" },
                { "si", "c65062d1a12c48e2e75ec2afab660f061cc0a420a4fbd16886950f2a9fa82d8b1981afe431a1b33c7439d46c47c419d9fb058a043c9e93fec1de9ca3f416b359" },
                { "sk", "259e4885f1306492359cecae98cfbcdf76a6e511c330c01ae0eaa6cc9c4dae810e277d629245bd88bb11fd8d169bc4f465519062520a79f1058a8b7e6fd5ba49" },
                { "sl", "4dbcbe6fb16f21adf0ea4bbdcd3cf16d4dff47ed45ebef3cc00af7b27a138d2f6101e15849ca5f76a9084aa2bfd300e1f573f92a737f9cf7fe8eb2362b6659b9" },
                { "son", "9a0d18d2e9c509be102871f42748a58ec61dc6d251d3bcb1cd1133ecde9858d6731f763381ea3d84b6772d6e95e30e2efe546fd7e0f82a127c040ab2b3bd22ed" },
                { "sq", "2e28e2365580993128362d406686d53ee0dedd89be20a0937f1503b138f8fe6a8a5b0a19b81d94019adbb430ae8e4c4802dd6322e4183ffaa977d42fafbe8260" },
                { "sr", "f464d895958a95e4f0a0330a118e6b55fa8e1ec5ce63ab608f4da41041ea54e6a222401c951c1930db7422d12eacb36d12e39dd5ff0889d7220a1605be852c50" },
                { "sv-SE", "2975fd71d072b87442a6371c9b64b026f581d587fbc5bd8e36f923baedd51fd50d19f3a600c44aae9c78ce54a1f56d24a6c0072ae7657babc0f36e9dc2fdea18" },
                { "szl", "4c0acc6ff5edc4733b58040bdc3ba2d72a999f06453fbb2d92b1871b08ef98ed34f4e5e36d3b04e7e1d20d2c2a67e73c3bbdaac68b49b45eadf84f6f148b40c2" },
                { "ta", "83a8fcc3a03b14d8495bf886f6447ec06fe98aa6617daa41a6ff8cb81d90787c78be2773b24617a12b76cdaf9dd58ee4545982fb7fef1655e41d80c3b834f6d8" },
                { "te", "ccfc2b56e89dd22e6b9b51bf698d04d9ec035be836f60504eb1ee74b2ecf1544c089d9e18a75d2d4f4cf759aed42db86af39bf371d07793a298a6eee3af98888" },
                { "tg", "190afb06b72ebe43a22ce80d33cf5ae42bd0bce501ba55851da06dc27f5d2579303b49c75b0fbea5f8eaa6cda56782a22609f1128bb8b53e83a88e69ff70a9df" },
                { "th", "a632552a1f3cb822f775ab49d5eac737902971907b34e585ee153443671000e75cf904a12c47281d819d6e85f5ca4591ee0af51630ba7409a16c661ebafecbd0" },
                { "tl", "07e96549140ef3cd113d2ac089484ae4059016169734c6fcf88973b0ce468271b4dd1538713f0dc9f1e96cfb9365e987c1ae0407821287dbd7a61445e3ab66d9" },
                { "tr", "b6cc5b7d7b90254cfc0798dc4c03942038c53d0ace07192099cab97841abd0623855ad2c7bf5172bca6594b49dfa6bbe0e5cf7b02a367930a56b6c064224a39e" },
                { "trs", "9a533689956f89ec77d1e1132fb33929b6aeda70e56fb6d4fcc3edf9fcd4f85ca90c9709520d50918fbde6a4691bc2cf0bc640dbcb09499de4cfa00c82cdd5d1" },
                { "uk", "15e5cb3798fa67878e48f4e173e09bbdbf224adb787a7145732544a1bef2eddea0267d39af21520770baa87eefe5485f2a6222db24ed567f26b4d81f8a9a41d8" },
                { "ur", "f5a8efdd5d869c8a5cceb4015304d7615cd16c19e4c8a0169e8146c36d8b6ff3d724f2210d57b9053863055ca248e23c823f7cdca455a4fd356e5a895a900585" },
                { "uz", "ed18a131c0c6640e82c293cb8afbf3b0cdac64d89a3e7b32af24ab0fa34e89e36c6614bd975fbcb828d87fde1d255c32b08346d5548ea5ec89ef4f9d1d401784" },
                { "vi", "1a1a67662b95fbb3b260f4ef5c4dc14b3794099f6836d01868b5c016e93ff0e04cded5c504b27db6e9b580e46f9b5aad2c34a739c14296b8b11a2126a918beba" },
                { "xh", "858528a6e454ce169cc27dc3ad5e210e812fdc325089942f724fb94e51504e139f26452135fe76cd53ee23a711d6e461460ea60642c29928a5df9a29b9235ce2" },
                { "zh-CN", "e39b248e40eeff4dc4c7b065ba5645bb3fbf4dd7ab5a4d25f6f2ea0f6051503706947551beda081155401c346c816d2d7533bc27f5a925c67e36a078287f6a22" },
                { "zh-TW", "1cb9636b754e20a3666b9f49483b19aed06c25df46506d4a28306fb05a3e7adf7ecc0741780ac229e9c947ec601c3dbd40188e771213c4accd5a76191ea88053" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/127.0b8/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "6569119f35655f039cbf7db6d2496938c1182d1b1063fbed7412752109a0e20c31a3ae9b5e7e79c16f09218857aeb81ddda7b884d9cae85ea049702def1c1b0b" },
                { "af", "fd7f71c3ea934362dbbc3e486667712dd5b57974f053ba375036551dee32c710ecbc97250286a557483c9df461f8fff6d31c139ea5bdcd874d3ec9cdf432f20d" },
                { "an", "a8edd9c131a5e269c2fdf1fbdf4837250ab9768b598f990865c85a1e64613f3d01d96b81bf3da97eb75b1e801043ebccbabb60a92e94a465b703d8fc3c2a2638" },
                { "ar", "f21b0a800919969f20c1ce4adde6b933a28b187e48131cfe6cca5779d8fb81e9604293e65c8ba1527b5c3791c8f09db5b00aa5bf2f7cbe275c99729d44cc2aa6" },
                { "ast", "73475d8d05bb4c5e27ca62a740e53aedbfc22844831aff57225c78fdee554d9b185c6080f22c09ed2e727694a4aea8dc80bb1d37415664eb80b2a0883d627a8d" },
                { "az", "1e06615d25d1af1ad29d0b78294d0c1db6af9f1c0f5efedfdeb43c014818e6681a7e8be223cdd37ad8a1b5d24c636e24616ae7dee1ae6879c432c8e655a5bb8b" },
                { "be", "5c4c06dc1749ac34e2f44d621456f726e6e11e93924b8e8ec092e176e432a659029f9dd0152a9b249c2e894e7109bd67663c1ee88e9812893676ecc5a18781e5" },
                { "bg", "c6458a376db624d9cc966da3afd14ceebcea624b27c0cd523484f22a25ab51057f4eb26fefbd25e540dd2ac8f575d1238c2b5eddcce7c7c200b8eae2f5c1825a" },
                { "bn", "df27f4345aa140902a53060ec6a453fc6fd1c6e2b34221adee3417a4de14089225496839ed385cb1f9b6fea7295c1d80d92ef0d192b43d0c3b1fb0c9c0609825" },
                { "br", "fa7fa8b0ad70da100ecdc9977a04330dc78aa2c833ca06f903809f14a2f669d5c3ebca0bcea5a7e16f0f88adcc49401a53422eb3e1fd89bf253c94955a76c7fe" },
                { "bs", "0b677b95b78dce9d805750156384c44d5a0454e457370f7f2f6bc0483369f9c2ba28b84ac09bccf2e0718937110e38ef5f0c9e2e7058f9a8607000129222f0ac" },
                { "ca", "2b7fd37256ad3da0682e799471b6fcb6144255f88d26e66b0c2d00e32aae0c6b88f62ca90c6811a1af0254f8e592a26131ed55d9d243777d2eac20ddd4b559f3" },
                { "cak", "9eb4c31fd23512fc5d86652bbf977cde88c675b2be699002761adf88e5ac217d7059ae1c9de197df2c2096fed16fcae9a1a236403e566c76eedeb134bd7df800" },
                { "cs", "6ba136852014f3f82a92c7d4b92fc1f24b864e52b0b7a6f839389f94d1553e7a439db14743ac49eecce3c97f22cd047f8d073f1eb7b48a8d1f2f6b21fb586a92" },
                { "cy", "2f7c4a54201782f9fbe86ec8e4cef2763cf533b4cecd4441e01ac161393572bba51f3b132c4df90b94f66d75d60a8989cafaf6a72b012cec008dd0d00b4f5253" },
                { "da", "da526a569d8bc4da6753d2dae63bdf8a864d75a89731075b9ef8f8af31e19d00fe40e59b5d7d04482061b2a93183284184f6c82069f9136fb4758c04d0628803" },
                { "de", "b95737be519eb3d21558df36e0d2fb2d247782f431a21b12e41fcb958fdde8ff5a3bd1a3c8605d20ada2faeccebc94b6d3d576dcbdaf3506079b88bb15634b00" },
                { "dsb", "6650f020f4d569ef5323ba6ba3390c40ab66b23e3a6d327a1aee69f5302e1a6151ad178021fa2dccc9822011131b04247bd16909d538ba7761b0c2f57efaeba1" },
                { "el", "2d67d470b773260beb30a7f2a4b747a767d849ac59bd1fad1967dad6a3084b8f903a10b2e4a815f71ffb43daa818e0773af156ff3d5e5629e872a364a53b9e90" },
                { "en-CA", "479dab487567a44f9b8b1ba5452ce133436713108da8f5959c339d9cca4376a5edc366876150406b7d55b5c2809a4de771c3d0c8ae0c0b13c800df93230a2ccc" },
                { "en-GB", "45698bbf7e66c529fbe8a35f3c755c3414dd2b99370e017a044819ea5b10f477e130fc36e5e4728dd904995db5088f2f4ae8a1e8112ccd82869ff12dc4ff8915" },
                { "en-US", "383768407b2f0a3a161a29ab4babec65735d0db137c12261dedb56301df0e7a0a13c280d0d04ac36e5161567a639bc7adf260f39a0e5052fe6aabc6832e1364a" },
                { "eo", "4c0e8ed3ecf39bef113d399de1c7e71036f23c0833258d5ebc0adb9e40eb110d2694ea530e5904e4b01b8a572d126b25b3eab914dae42ddc7063b21014d3892e" },
                { "es-AR", "4615a14b1861529887ec7cbbf411ba1df8451177d34cea5e24419a53aa576a383345d835e3260c6e80907bcccdf0816c077952822c76d0ba3f48c549837b5863" },
                { "es-CL", "3c1599a5a27bc2336777eb9ccf79d03cb7bf7d15bcb8fc28fbe66e52d56192ea5c2f8c70a53278b35e40540b5faa5d3478c34c464fcd563c849cddd015f7924d" },
                { "es-ES", "11eeb836e53abd7e0757418a5fcd50f0743e26f24a0736222ed5a9a3b5d750bbba083ef73e51f37e9eee9fbe687c6fe9a32a2508c0e0c513d62dd613c6f5c64c" },
                { "es-MX", "350c92197bc512bb2c1383174d6976c379df211d8386f8be38a51dce6f0a252be42e8b6f67e398006acec01c6588116988dcbd55b310aa11079dc97e16a23686" },
                { "et", "d4006f12ce2cf3e3c8f5c7c2010a9b7ef8119b355092b312a6ca70bf26ecd9c6067b5286be46b2191faa9c3ea178021603a217e3ef28083700f70b414f577ac7" },
                { "eu", "9388256a1e49b9da516e44b44778adcf9caf10198bafe67c59e1b804504825f86e3169cd8ee6cff176d5b7a55d34a2e5187472cbd969ac6e26b859f9ae217ba7" },
                { "fa", "d1f1ca310790a6d74ed4b52b81f44dc7c3454a8d6d8d97d6e12bd20cd6a32dce6af5fc7da48624e2372fb34ffb67c8dd9c3e853a99d9e23a3e4256064f3da35c" },
                { "ff", "720d03b6286273319d33601e75600f93e7685c898da91efd4bbfa9e62ec5abdee9e3da8ad423d9316119d9ebf0ec6516d7bc48bfa68e1a6c58293fd87a567e5a" },
                { "fi", "0b0f1ab1b62111e5fdc9062cfcf699adf209fc96f67bbc56ad75a7bcd065a35069e830d42615f5f4b8c1ba2218cf0cc1210d3d121ecb96394a5a1157f11747ec" },
                { "fr", "df7cb854410f038fc1051c56678f1d792df7906b1469d35b94e450b3f34fe7baf4e752f74f9aaa961488f76a31567bc5be22371c5ac9225e9d0103186ed35f47" },
                { "fur", "5762e7bdafea2236523697f855a1db28d5a9b554c6ad8cdc5c3343bb37a60df1a50c9a7b86874c76d4bcc6d951cd6405233e8bc7080f6d179e08c4e9660c969b" },
                { "fy-NL", "16122f46d328275b2aa6349eb383cfb513cfcfe2fc3ef70ae2339cfbb4cc7a12022b68bed2d0153cf13cbec75bf7a52b69a469d2691466ac8c358864dc84a18c" },
                { "ga-IE", "9578aee307f30b58f0e153b1099df78ed932bd85e9035772169bb799cc2af2b223b898a7c6876829a01fdf6a88092b51908e1adeaeddae20474a6f6440d74bb8" },
                { "gd", "b4b213d421a5b3b008f33f17860bbd96bedb6bf0a98c3901041b3c8ed0374c2dd0c7d82854be0962cc055584e81e9832962df1f884c0acded2c58f7bf919311a" },
                { "gl", "67092c1c65708e58cb3210808265e2c8416e647bc9e0832163421676535e3db37e593a67bc812320086fa974dcda44098443446e26cdd43b8afca4f2fdfc1280" },
                { "gn", "4c1efbaa6392f3ef8e6acb475671f8d1dd860ccfedc949b205acf30081f7449150f34ba2396a06bc6455435c123d6562666420f6c9fdf98785f350d3a736ca25" },
                { "gu-IN", "4ddaa9306d7dbab2b94d9da3fb93f7706b318c9753e097d34c3261b3a18a5d2badceaa00aa9f13f43f61e22944c5e5e9a8b489e692418301a8a023d995f57b8f" },
                { "he", "30e71654e6c0a2fd5c62763bdf1dcc133c6ca5867cf90aa6d7412d9f59dc6e127653b17d7d0efb5eafdd56bf0c9e7915591f68a2180f408e1c758be07d573991" },
                { "hi-IN", "d0b16ea1038d043ee3dd2bc7c65304e1d69a7897f4d111733b55b84a72417722692b19b3bd8f3a319b733ee2984f069e7dc7807e193b2bd4e9e784b61dbd1e3a" },
                { "hr", "7d343acc793603aa886c0e9a824a5979f00819f58dc2991aedd946eb618455a4a692f6e27fe531ab79169756ac55266c7f14f482282f27c7bae8d6fb875c3f0c" },
                { "hsb", "4db9bb6381120208559baaa42e43d5b790c858a3ecc7a5d696602dd56e602c9a58fcf4bbeae9d2cbfc3364ca7fa357d0a202eeed8298ba3351acdedde4aa2794" },
                { "hu", "1c29bfad4d3734f2c53282b582ae0281c2ba4be577181e68b52013b74efa0a729b4ad0d1dd6b9904f8fa66439b4f0fce5e9df20a2a7ff9c897a6d6f3c6bafaea" },
                { "hy-AM", "cc0844fd96600ba6ab9ec2159b54375676597831757ee8322444f60dd978ec53e0d87524e7a86eab0da2d405410e61796f3078777e2f95d1c40671b1015fd67c" },
                { "ia", "788ae32d68a7a655a0bac4d71c2d9d438472f67c95c94b3ad8bed89f1f2fdbce808ef678ed2a5f2c0e4f830bae2b9f7181427ca6fcf163cd0a53b76a5656ebc2" },
                { "id", "b9ebf6f58ffbb5003bb93582cafef86b48959f49375a0f755dc71878650f96be4e4db8486d94c9436a8aec149640b0bbd3a0a7069a8d61aa646db792c48a3b99" },
                { "is", "16e03c773f98ee6ec5b3314cebc04cedbe60286a5ce7723f000d248e0083d3d9bf13ad7ca17463d3077ec7940fd8ed953a86a5d2d9a1c540333227c35935d8e1" },
                { "it", "0b818c2ff88b081ea31f8b67a82a70b8e13def60050ba4ef4b7ce46b763570f8a60a3bd143a8dc6c4cfb486fddf3db70a6ce9b0219c6344db7c9f3107a77c21c" },
                { "ja", "0b7198c68e0d0fb2c8873cc61f8f710450bc26eda1d14677c6e32fdb6f38a94e2185f3e1409a75c4b87493c64921e81a298168688829a30b4f88fa03a7bce430" },
                { "ka", "28b661f506ccc61524f2deee469843e8432aefe61211911cb8a03fcb719e025060474dff340238bb34648723d03b7a7073f99f8973d95e445efc2c094a31bb52" },
                { "kab", "e12481f2fab6cb738d6184cceaced876c9953e1cfbb3cb627a70c7814d76040cb641e1fc01f17c43355b34d41a0c570dfba328c094879af4473ea6f3472bf055" },
                { "kk", "5517f80673c411defff3c812026d622721465c03e9adbfaff380f4dff1116a26ee1886f529420b35af21036f25861014f96ff8be46193030bb892c3ed91e56e7" },
                { "km", "ac8e4bed71f1e3b376419ed695918f6096dedae0eecb2b81a83a66a15f4f8d6a477ef37f74bac5a466048e836dd81f05d555a553770fc8b78e6790b61a98cbf7" },
                { "kn", "f1c29804793419357c0b52ec2a1e138b1785869aaa5a5266f090cae23570e6a430c80a599093051eecd6c28441b5f0faf1b869b14ae3296ba7360f86f5253f6f" },
                { "ko", "685f53de2a6ebf05e39df780cb5dc95469ea91bd4a08a8ec54c4ee24de878ed8c46a91bcf17d6286d908fbc5304355be7d017935d12aa9c83eaa7a57e5201563" },
                { "lij", "2445f6b38414ebe776b1ed1f74c002a90c3a6cfea265d8812650816b8e0b39bd0d966b72473c0dfece6cb67d8e3ba4f3e73e74abb9580c1a98fd6fe3c9941711" },
                { "lt", "44315957c7222d363042ba47134cf14cee443a0f0036d8f986864d374c1121270442db5ec8e1b81b0429e4bbd414fca49688675747c0b0d0fa0fd0a11d3b50ba" },
                { "lv", "0aa7bd01984929376c33e29bd1cbf95ef0f4c603f2b2b3e63598b946cdb010f42a8fc083c1ffa1675e25e96c3652a3f4c46b7a876ce623bb4c0fd86e23670d29" },
                { "mk", "4954efe7bfd1f4b11a89c64b6827b920d5b08173eeaa36d07e9a3161d54e6803d5058a3068f50adea4ca892ca9f94957429876b0e79306659c2ac63250382d31" },
                { "mr", "0c385f09783c405f5870eb9d62c7296fb8811cae051681d47a0b8f00c823d3a73230b24ccdae1d6a89ba8ede8a54be29a87fcd4f80506a144138500fc7c51cc8" },
                { "ms", "6a89a2aa57c4a37d7f03d25f091282a6784528d56a07865259357689d4f027d74f9e92923c6d1932367ef62fbbfd7e5b7aa0a38e037f3730a26250d9d6f7bff7" },
                { "my", "f605fd5d79450f1429435656f5e24d0cd0dc88f9f7f83cb2eb00df0b77fc4abf9d43d57595d93ac6f97e089b17f9d3fbb3903ba8eb47dd7f4d204d6702139eb5" },
                { "nb-NO", "6b408919214ef8fb994e0baa9db96ce807bdf35aae7aaae0725d8aa66c49c13f8006d0fc3c016317cf76161554b45104370468087f16796368a49ec80feec772" },
                { "ne-NP", "5960f0ccf6d7deb8b8aff4cb0a24c4a9dd53260945270c701e8d319efe63cbebc51f0d0b1e8a8034ee9cdae2cee09ca31fb470b4faa9522632164e1cde9dc7d5" },
                { "nl", "f33313a7bfd27ffbba3a0cd2ba875e93dde8cf1c0f5d236b19ebf7125f723659a7a8dbd0d81f27dfb577ea31415b2cf0fc01abc93fb9dbfd9b31d624358654bf" },
                { "nn-NO", "2cc10c41759d0ce8eb1c99ac4059c05b52c7b651485efdf294eb973861e85f8cd8c13821d64eebdcd335cc67eca17b37d8df22b9275676ba3a9b3e8767a212c6" },
                { "oc", "9ffd5fe1c0c25c61da0a54f5913b6bc2892474a00bb2fea8abfe25e5ce8d60897eed064b51a0b4d19b04e875cd699cf0f12d278665505614aaddcaf147eb96da" },
                { "pa-IN", "475180abfcdd47fc95c11a9b30f792618c9d4b67d6442c118c303796ccd7e15fd15879247292028a904066b72834d433254deee546625ebf396ec7832d9cf4f8" },
                { "pl", "7590e5836673863d1e4fd18a27b00d3a639e8d2c66e992fd476fa5b72b5f58ce96a179d86aa4079e92af756a2a2533704fa23a1d33b153a42e99da03ef8e79a1" },
                { "pt-BR", "3c630c985a74c240e55c560cce60405b98f716ba580eaaeea2dd52dafac31dfca8fadaffaa813baf9d966760735be42f5b6797f14671d05ce9a9dcf8962bb2d3" },
                { "pt-PT", "6a6d6706046a99744d43bacf939d247c72a23d98daa74f895d2b1d595e73760e91b7337cd4229ef622bde4590686b0a525b61f140f86c358699afe8b0e01be3b" },
                { "rm", "9125b92e93367b4587d39b14524d9ebfcb1ec50b0ee7d635743d31c42674bbe3d5548fdff86c643b43d49b8514bdec3e488a669e0f434ef13ff2205c917e2d3b" },
                { "ro", "bedc6d582790ce13ec9d34458cdf0cc19233bbbcc5028765273b98abd30a5412eb271929f21f220d6a8e1481ba7dd4d2666cf71469bc262a3718b65f2391612d" },
                { "ru", "ea50ce8b8261fd15582754bb77d99a5eaccc26995adc6403ec88aaf2f0e43596389e9b9acb807a7cb3cad3add685a2899464055a69972f44c090f437f97d0fe4" },
                { "sat", "3bb8bdc331832cb0e87b08bc32e8854ef3db51d77930b29fb652f88600340ce358e6cecae3c6bd0acf53a96260820aef2a9ac22d753c0c04f3833f1e35f4a414" },
                { "sc", "ac566567071ca3f0af8ae6820b39aa83ed108072d0fd0fc620626eef31c798238354d6209929d3a5cb348a532aa3712d58537b236b50fe4965722171d1af43e2" },
                { "sco", "74f3f20d282d70fbe0718d362095448f803479f3586a42fbaa39775ad48cb440b31c05429d56072fb1781c6de4bc56b5248a39c4e6dc80ab17bba933faa26150" },
                { "si", "96705939d469826294ea56d7fb4b55b558362f9004e77cfe45bf7deec73bfa0409891efe8596d910f17bf4011ff209bcbe8f6a545ae9e37a7406e5f2a46d8770" },
                { "sk", "e66df8ae964145a1d0f2aa33aa83bd2deb59cf466ff57361517ec3afa26505b63dad0a466c34ed27b61065677612a74c84a55ddb1c8d595aa7e2d0bfa2fe5dfe" },
                { "sl", "c3cc90315a738f0d65c9984ef998fba33cfadacf48d1315dad2443f1844eadf59681d3d2712857262a75c0833fd0ca3a96cf3562afd39d6e546ba7498dffd0f4" },
                { "son", "3fbb6d4975006403ecd17b0e98a5fa4ee75acacd4d485c212571be933d068e7affbf0136ad4fb74cbf09527455f0d18e90178e5e7b14b6f4c88d9afe12f96bba" },
                { "sq", "7bc30e44f85a41adbd322e628d23fe7b2a439af700fe843e7731ede4a7787b7992851ac350d7e9605366609e49870ae9df31c3a03eeef0c8a6d84f9bdeba5ad1" },
                { "sr", "f426dccf739f121b1e84bd8097ade57cc8a28fab7f7a317612c868943c12731f2a9a11990420129cfddce41f948b28ee8a8727b13006c6ce78f903cde21b159d" },
                { "sv-SE", "151fffd6386c5fec0d2ff6766fc8296468558a45754231b40be52a81b39c049b2f96c2985303bb10b381aedc9929cde1bb50a9036beeab81fa19f05453c79b9b" },
                { "szl", "5e09d15f2e974bfe32406bd5f9d3cff652ba691e42430be01d7f7560df9cc5c81931722db835f23b123ddc42cda9b416038a390623cdbcbdf8e256ab9fa96592" },
                { "ta", "950f601c9f515d4718587631a048b6cf6257e8e0f2d9f23b2d4b86744e738306fe9f04ba436c7bd213a7ccb9d2b0319a82c4f4096772c3f0d97e6671dcb120b1" },
                { "te", "63181549b7e51747ef058fc04832cc96fc308cce1df37ff887ad5f5b0adcc4c772cfffacbb472965f16eae7bb4279a44bab8502e5a6d47469cd78a5a49a13c34" },
                { "tg", "d66dfeb19c5aad8007da4c715f5da020e57472c6d0b8781b97503af8569be99475bbf05da29a1b04924d6e4701de1e385ca69d17b5f64b76abbd0c4880c0bd8c" },
                { "th", "2a6ba7145399082b8d5bc325f138a529fd48d4d8e71c8c99a028905ef2992de56392266be95353e6787423957e0b9b8846b27cc2c41a9feb3c9bd6c44a0d6809" },
                { "tl", "dc1c1cd49999d59e9822557b08b2ade1f811c5408ef5609f657403541d28c6073b951acb96df5b745e75cad05cc3f93416f89f09e743288ada426cd62e812927" },
                { "tr", "ca67597de504ae6d25679ab9401dae4d4a8d26b0e02f622e119903b723f0c67b68ce381c2accf3ec8c9f5a5b45348d492baf6011f30bb538cdcd30c6f406b8b2" },
                { "trs", "00566487db213eb549b6727e111369dc2769f1d7a975409898a75d1ad7cecf03608430b11a53612e072e6b8b45d14a03c808a75e1df544122dc1b6142c22357e" },
                { "uk", "0d068627066625f706c92bb572756787f80764376ce422c32b3c2fa6e1c1abcbf6640aa4d59e60a98cb9c7bda228fa4b95618fab71db98ab0c832858e0ade47a" },
                { "ur", "28534f8ab8afaa5cf884a592f7ce2b831fed2374ab85a382bb2705021792d3f1186f6eae3cf4653bf99b2155bf7117212a954915607c42d99adaf73213a05c9f" },
                { "uz", "827dbac18eee8daaa9e121190420549e86f9ba3409dac801cd0fcfe7ab05ab1a742fa03f37042bf30c60770d51cd46facd8ffb0d4204a2919417798c825cdb4f" },
                { "vi", "7a4cb9b5f960ddbc0fb5e1c97f995de7cf315408b91d21dcc56afac263ecb17823e1f1de7700123dcc167e9496efe3e40fb438c9c733e99f3d08697cb8cc4b93" },
                { "xh", "1f45ce5ec91626ce92941bc55c9b66e548de16c328ad85a35fb9a3a50d71474ae90a933a8563437ae4f96f4d7ce382973d397561594209cf25ca351d3082e049" },
                { "zh-CN", "0dd1bc24cfb0241aad3cf55be9ae746b331e82b64e3fa64805e3897d41bc3c4c07c23fd300b44249af7077a100f7b0adff78b7ca0c9c5e0fc8077b2c30ce9783" },
                { "zh-TW", "ec46f41bc2da246cb4aa7a2888e2dfa452347b535725164b82b4b6b4e13ba6676a8796ac50924708d652d1b52ddc421cbdbef0e40ffcd1814bb88e544266563a" }
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
